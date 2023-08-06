using FirebaseAdmin.Messaging;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Org.BouncyCastle.Asn1.Ocsp;
using PTApplication.Models.DBContext;
using PTApplication.Models.ORM;
using PTApplication.Models.ViewModels.Tags;
using PTApplication.Models.ViewModels.Users;

namespace PTApplication.Controllers
{
    [Route("PTAPI/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class RequestReplyController : ControllerBase
    {
        private readonly PTAppDBContext db;
        public IConfiguration configuration { get; }
        public RequestReplyController(PTAppDBContext _db, IConfiguration _config)
        {
            db = _db;
            configuration = _config;
        }
        [HttpGet]
        public Response Get()
        {
            return new UserViewModel(db, configuration, HttpContext).GetMyRequestReply();
        }

        [HttpGet]
        public Response GetForApproval()
        {
            return new UserViewModel(db, configuration, HttpContext).GetMyRequestReplyForApproval();
        }

        [HttpPost]
        public async Task<Response> Add([FromBody] RequestReply requestReply)
        {
            Response response= new UserViewModel(db, configuration, HttpContext).AddRequestReply(requestReply);
            if(response.isSuccess)
            {
                User user= db.Users.Where(x => x.userID == requestReply.clientID).FirstOrDefault();
                if (user != null)
                {
                    await SendNotification(requestReply.pTID, "You have a new request", (String.IsNullOrEmpty(user.fullName)?user.fullName:user.fullName)+" wants to contact you for training.", db);
                }
            }

            return response;
        }
        [NonAction]

        private async Task<Response> SendNotification(Guid? uID, string title, string body, PTAppDBContext database)
        {
            Response response;
            try
            {

                response = new Response();
                User CurrentUser = database.Users.Where(x => x.userID == uID).FirstOrDefault();
                var credentials = GoogleCredential.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bellfit-3840f-firebase-adminsdk-7yovt-184d441605.json"));

                //var credentials = GoogleCredential.FromFile("/bellfit-3840f-firebase-adminsdk-7yovt-184d441605.json");
                if (FirebaseApp.DefaultInstance == null)
                {
                    FirebaseApp.Create(new AppOptions
                    {
                        Credential = credentials,
                    });
                }


                // Create notification message
                var message = new Message()
                {
                    Notification = new Notification
                    {
                        Title = title,
                        Body = body
                    },
                    Token = CurrentUser.firebaseToken
                };

                // Send the notification
                var messaging = FirebaseMessaging.DefaultInstance;
                string __response = await messaging.SendAsync(message);

                // Handle the response
                response.responseObject = $"Successfully sent message: {response}";

            }
            catch (Exception ex)
            {
                response = new Response();
            }
            return response;
        }

        [HttpPost]
        public Response Approve([FromBody] RequestReply requestReply)
        {
            return new UserViewModel(db, configuration, HttpContext).ApproveRequestReply(requestReply);
        }

        [HttpPost]
        public Response Reject([FromBody] RequestReply requestReply)
        {
            return new UserViewModel(db, configuration, HttpContext).RejectRequestReply(requestReply);
        }
    }
}
