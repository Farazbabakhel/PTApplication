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
    public class PackageController : ControllerBase
    {
        private readonly PTAppDBContext db;
        public IConfiguration configuration { get; }
        public PackageController(PTAppDBContext _db, IConfiguration _config)
        {
            db = _db;
            configuration = _config;
        }
        [HttpGet]
        public Response GetByKey(string PackageID)
        {
            return new PackageViewModel(db, configuration, HttpContext).GetPackageByKey(new Guid(PackageID));
        }
        [HttpGet]
        public Response Get()
        {
            return new PackageViewModel(db, configuration, HttpContext).GetAllPackages();
        }
        [HttpPost]
        public Response Add([FromBody] Package package)
        {
            return new PackageViewModel(db, configuration, HttpContext).AddPackage(package);
        }

        [HttpPost]
        public Response Update([FromBody] Package package)
        {
            return new PackageViewModel(db, configuration, HttpContext).UpdatePackage(package);
        }

        [HttpPost]
        public Response Delete([FromBody] Package package)
        {
            return new PackageViewModel(db, configuration, HttpContext).DeletePackage(package);
        }
    }
}
