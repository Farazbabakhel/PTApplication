using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PTApplication.Models.DBContext;
using PTApplication.Models.Interface;
using PTApplication.Models.ORM;
using PTApplication.Models.ViewModels.Users;

namespace PTApplication.Controllers
{
    [Route("PTAPI/[controller]/[action]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly PTAppDBContext db;
        public IConfiguration configuration { get; }
        private readonly IMailService mailService;
        public AccountController(PTAppDBContext _db, IConfiguration _config, IMailService _mailService)
        {
            db = _db;
            configuration = _config;
            mailService = _mailService;
        }

        [HttpPost]
        public object Login([FromBody] User user)
        {
            try
            {
                LoginViewModel model = new LoginViewModel(db, configuration,HttpContext);
                
                return model.Login(user);
            }
            catch (Exception ex)
            {
                return new Response(ex);
            }

        }
          
        [HttpPost]
        public Response PTSignup([FromBody] User user)
        {
            try
            {
                user.accountType = "PT";
                return new SignupViewModel(db, configuration, HttpContext).Signup(user);
            }
            catch (Exception ex)
            {
                return new Response(ex);
            }
        }
        [HttpPost]
        public Response SignupClient([FromBody] User user)
        {
            try
            {
                user.accountType = "Client";
                return new SignupViewModel(db, configuration, HttpContext).Signup(user);
            }
            catch (Exception ex)
            {
                return new Response(ex);
            }
        }

        [HttpPost]
        public Response VerifyEmail([FromBody] User user)
        {
            try
            {
                return new SignupViewModel(db, configuration, HttpContext).VerifyEmail(user);
            }
            catch (Exception ex)
            {
                return new Response(ex);
            }
        }

        [HttpPost]
        public Response GenerateOTP([FromBody] User user)
        {
            try
            {
                return  new SignupViewModel(db, configuration, HttpContext).RegenerateOTP(user);
            }
            catch (Exception ex)
            {
                return new Response(ex);
            }
        }

        //forgot
        //resend oto

    }
}
