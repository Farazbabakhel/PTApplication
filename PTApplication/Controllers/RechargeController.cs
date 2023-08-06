using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PTApplication.Models.DBContext;
using PTApplication.Models.ORM;
using PTApplication.Models.ViewModels.RechargeVM;
using PTApplication.Models.ViewModels.Setting;

namespace PTApplication.Controllers
{
    [Route("PTAPI/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class RechargeController : ControllerBase
    {
        private readonly PTAppDBContext db;
        private readonly IConfiguration configuration;
        public RechargeController(PTAppDBContext _db, IConfiguration _configuration)
        {
            db = _db;
            configuration = _configuration;
        }
        [HttpGet]
        public Response GetRechargeHistory()
        {
            return new RechargeViewModel(db,configuration, HttpContext).GetMyBalance();
        }
        [HttpGet]
        public Response GetRechargeHistoryByID()
        {
            return new RechargeViewModel(db, configuration, HttpContext).GetMyBalance();
        }
        [HttpGet]
        public Response GetUserBalance(Guid userID)
        {
            return new RechargeViewModel(db, configuration, HttpContext).GetUserBalance(userID);
        }
        [HttpPost]
        public Response RechargeBalance([FromBody] Recharge recharge)
        {
            return new RechargeViewModel(db, configuration, HttpContext).RechargeMyBalance(recharge);
        }

        [HttpPost]
        public Response DeductBalance([FromBody] Recharge recharge)
        {
            return new RechargeViewModel(db, configuration, HttpContext).DeductBalance(recharge);
        } 


    }
}
