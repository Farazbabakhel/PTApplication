using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PTApplication.Models.DBContext;
using PTApplication.Models.ORM;
using PTApplication.Models.ViewModels.PTBio;
using PTApplication.Models.ViewModels.Tags;

namespace PTApplication.Controllers
{
    [Route("PTAPI/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class TagController : ControllerBase
    {
        private readonly PTAppDBContext db;
        public IConfiguration configuration { get; }
        public TagController(PTAppDBContext _db, IConfiguration _config)
        {
            db = _db;
            configuration = _config;
        }
        [HttpGet]
        public Response Tags()
        {
            return new TagsViewModel(db, configuration, HttpContext).GetAllActiveTags();
        }

        [HttpPost]
        public Response Add([FromBody] Tag tag)
        {
            return new TagsViewModel(db, configuration, HttpContext).AddTags(tag);
        }
    }
}
