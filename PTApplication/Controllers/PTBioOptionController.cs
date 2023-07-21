using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PTApplication.Models.DBContext;
using PTApplication.Models.ORM;
using PTApplication.Models.ViewModels.PackageVM;
using PTApplication.Models.ViewModels.PTBio;

namespace PTApplication.Controllers
{
    [Route("PTAPI/[controller]/[action]")]
    [ApiController]
    public class PTBioOptionController : ControllerBase
    {
        private readonly PTAppDBContext db;
        public IConfiguration configuration { get; }
        public PTBioOptionController(PTAppDBContext _db, IConfiguration _config)
        {
            db = _db;
            configuration = _config;
        }

        #region Bio Question Option

        [HttpGet]
        public Response GetByKey(string optionID)
        {
            return new PTBioViewModel(db, configuration, HttpContext).GetOptionByKey(new Guid(optionID));
        }
        [HttpGet]
        public Response GetByQuestionID(string questionID)
        {
            return new PTBioViewModel(db, configuration, HttpContext).GetOptionByQusetionID(new Guid(questionID));
        }
        [HttpGet]
        public Response Get()
        {
            return new PTBioViewModel(db, configuration, HttpContext).GetAllOptions();
        }


        [HttpPost]
        public Response Add([FromBody] BioOptions bioOptions)
        {
            return new PTBioViewModel(db, configuration, HttpContext).AddQuestionOption(bioOptions);
        }

        [HttpPost]
        public Response Update([FromBody] BioOptions bioOptions)
        {
            return new PTBioViewModel(db, configuration, HttpContext).UpdateQuestionOption(bioOptions);
        }

        [HttpPost]
        public Response Delete([FromBody] BioOptions bioOptions)
        {
            return new PTBioViewModel(db, configuration, HttpContext).DeleteQuestionOption(bioOptions);
        }
        #endregion
    }
}
