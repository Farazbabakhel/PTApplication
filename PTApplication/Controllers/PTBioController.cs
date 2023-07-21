using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PTApplication.Models.DBContext;
using PTApplication.Models.ORM;
using PTApplication.Models.ViewModels.PackageVM;
using PTApplication.Models.ViewModels.PTBio;

namespace PTApplication.Controllers
{
    [Route("PTAPI/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class PTBioController : ControllerBase
    {
        private readonly PTAppDBContext db;
        public IConfiguration configuration { get; }
        public PTBioController(PTAppDBContext _db, IConfiguration _config)
        {
            db = _db;
            configuration = _config;
        }

        #region Bio Question
        [HttpGet]
        public Response GetByKey(string questionID)
        {
            return new PTBioViewModel(db, configuration, HttpContext).GetQuestionByKey(new Guid(questionID));
        }
        [HttpGet]
        public Response Get()
        {
            return new PTBioViewModel(db, configuration, HttpContext).GetAllQuestion();
        }


        [HttpPost]
        public Response Add([FromBody] BioQuestion bioQuestion)
        {
            return new PTBioViewModel(db, configuration, HttpContext).AddQuestion(bioQuestion);
        }

        [HttpPost]
        public Response Update([FromBody] BioQuestion bioQuestion)
        {
            return new PTBioViewModel(db, configuration, HttpContext).UpdateQuestion(bioQuestion);
        }

        [HttpPost]
        public Response Delete([FromBody] BioQuestion bioQuestion)
        {
            return new PTBioViewModel(db, configuration, HttpContext).DeleteQuestion(bioQuestion);
        }
        #endregion


        


        [HttpGet]
        public Response GetQuestions()
        {
            return new PTBioViewModel(db, configuration, HttpContext).GetAllQuestionWithOptions();
        }
        [HttpPost]
        public Response AddBioQuestion([FromBody] BioQuestion bioQuestion)
        {
            return new PTBioViewModel(db,configuration,HttpContext).AddBioQuestion(bioQuestion);
        }
        [HttpPost]
        public Response AddBioOptions([FromBody] BioOptions bioOptions)
        {
            return new PTBioViewModel(db, configuration, HttpContext).AddBioOptions(bioOptions);
        }

    }
}
