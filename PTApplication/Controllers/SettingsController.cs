using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PTApplication.Models.DBContext;
using PTApplication.Models.Global;
using PTApplication.Models.ORM;
using PTApplication.Models.ViewModels.PTBio;
using PTApplication.Models.ViewModels.Setting;
using PTApplication.Models.ViewModels.Tags;
using System.Net.Http;

namespace PTApplication.Controllers
{ 
    [Route("PTAPI/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class SettingsController : ControllerBase
    {
        private readonly PTAppDBContext db;
        public IConfiguration configuration { get; }
        public SettingsController(PTAppDBContext _db, IConfiguration _config)
        {
            db = _db;
            configuration = _config;
        }


        [HttpGet]
        public Response GetSetting()
        {
            return new PTSettingViewModel(db,configuration,HttpContext).GetMySettings();
        }

        [HttpGet]
        public Response GetSettingByUserID(string userID)
        {
            return new PTSettingViewModel(db, configuration, HttpContext).GetUserSettings(new Guid(userID));
        }

        [HttpPost]
        public Response Add([FromBody] PTSetting setting)
        {
            return new PTSettingViewModel(db, configuration, HttpContext).AddUpdate(setting);
        }

    }
}
