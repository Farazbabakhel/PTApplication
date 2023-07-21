using FirebaseAdmin.Messaging;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MimeKit;
using PTApplication.Models.DBContext;
using PTApplication.Models.Global;
using PTApplication.Models.ORM;
using PTApplication.Models.ViewModels.Tags;
using PTApplication.Models.ViewModels.Users;
using System.Net.Http;

namespace PTApplication.Controllers
{
    [Route("PTAPI/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly PTAppDBContext db;
        public IConfiguration configuration { get; }
        public UserController(PTAppDBContext _db, IConfiguration _config)
        {
            db = _db;
            configuration = _config;
        }

        [HttpGet]
        public object GetProfile()
        {
            try
            {
                SignupViewModel model = new SignupViewModel(db, configuration, HttpContext);

                return model.GetUserByUserID();
            }
            catch (Exception ex)
            {
                return new Response(ex);
            }

        }

        [HttpGet]
        public Response GetUserProfile(Guid userID)
        {
            try
            {
                SignupViewModel model = new SignupViewModel(db, configuration, HttpContext);

                return model.GetUserProfileByUserID(userID);
            }
            catch (Exception ex)
            {
                return new Response(ex);
            }

        }

        [HttpPost]
        public object UpdateProfile([FromBody] User user)
        {
            try
            {
                SignupViewModel model = new SignupViewModel(db, configuration, HttpContext);
                return model.UpdateProfile(user);
            }
            catch (Exception ex)
            {
                return new Response(ex);
            }

        }

        [HttpPost]
        public object AddTags([FromBody] User user)
        {
            try
            {
                return new TagsViewModel(db, configuration, HttpContext).AddUserTags(user);
            }
            catch (Exception ex)
            {
                return new Response(ex);
            }

        }

        [HttpPost]
        public object AddUserBioAnswer([FromBody] List<BioQuestion> userAnswer)
        {
            try
            {
                //
                return new TagsViewModel(db, configuration, HttpContext).AddUserBioAnswer(userAnswer);
            }
            catch (Exception ex)
            {
                return new Response(ex);
            }

        }

        [HttpGet]
        public Response GetUserTags()
        {
            return new TagsViewModel(db, configuration, HttpContext).GetUserTags(Utilities.GetIdentity(HttpContext).userID);
        }

        [HttpGet]
        public Response GetAllPTs()
        {
            return new UserViewModel(db, configuration, HttpContext).GetAllActivePTs();
        }
        [HttpGet]
        public Response GetAllClients()
        {
            return new UserViewModel(db, configuration, HttpContext).GetAllActiveClients();
        }

        [HttpGet]
        public Response GetConversions()
        {
            return new UserViewModel(db, configuration, HttpContext).GetConversions();
        }

        [HttpGet]
        public Response GetClientCredits()
        {
            return null;
        }


        [HttpGet]
        public async Task<Response> SendNotification()
        {
            Response response;
            try
            {
                
                response = new Response();
                Guid? uID = Utilities.GetIdentity(HttpContext).userID;
                User CurrentUser=db.Users.Where(x => x.userID == uID).FirstOrDefault();
                var credentials = GoogleCredential.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bellfit-3840f-firebase-adminsdk-7yovt-184d441605.json"));

                //var credentials = GoogleCredential.FromFile("/bellfit-3840f-firebase-adminsdk-7yovt-184d441605.json");
                if(FirebaseApp.DefaultInstance==null)
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
                        Title = "New Message",
                        Body = "You have a new message."
                    },
                    Token = CurrentUser.firebaseToken
                };

                // Send the notification
                var messaging = FirebaseMessaging.DefaultInstance;
                string __response = await messaging.SendAsync(message);

                // Handle the response
                response.responseObject= $"Successfully sent message: {response}";
                
            }
            catch (Exception ex)
            {
                response=new Response();
            }
            return response;
        }
    }
}
