using PTApplication.Models.DBContext;
using PTApplication.Models.Global;
using PTApplication.Models.ORM;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;

namespace PTApplication.Models.ViewModels.PackageVM
{ 
    public class PackageViewModel
    {
        private readonly PTAppDBContext db;
        private readonly IConfiguration configuration;
        private readonly HttpContext httpContext;
        public PackageViewModel(PTAppDBContext _db, IConfiguration _configuration, HttpContext _httpContext)
        {
            db = _db;
            configuration = _configuration;
            httpContext = _httpContext;
        }
        public Response GetPackageByKey(Guid packageID)
        {
            Response response;
            try
            {
                response = new Response();
                Guid? UID = Utilities.GetIdentity(httpContext).userID;
                response.responseObject = db.Packages.Where(a => a.packageID==packageID && a.isActive == true).FirstOrDefault();
            }
            catch (Exception ex)
            {
                response = new Response(ex);
            }

            return response;
        }

        public Response GetAllPackages()
        {
            Response response;
            try
            {
                response = new Response();
                Guid? UID = Utilities.GetIdentity(httpContext).userID;
                List<Package> Package = db.Packages.Where(a =>  a.isActive == true).ToList();
                response.responseObject = Package;
            }
            catch (Exception ex)
            {
                response = new Response(ex);
            }

            return response;
        }

       

        public Response AddPackage(Package package)
        {
            Response response;
            try
            {
                response = new Response();
                package.createdBy= Utilities.GetIdentity(httpContext).userID;
                package.creationDate= DateTime.Now;
                package.priceAfterDiscount = package.price - ((package.price * package.offPercent) / 100);
                if (String.IsNullOrEmpty(package.code))
                {
                    Package PackageCode = db.Packages.Where(a => a.code == package.code && a.isActive == true).FirstOrDefault();
                    if (PackageCode == null)
                    {
                        package.packageID = Guid.NewGuid();
                        db.Packages.Add(package);
                        db.SaveChanges();
                        response.message = "Package Added Successfully.";
                    }
                    else
                    {
                        response.isSuccess = false;
                        response.status=(int)ResponseStatus.Conflict;
                        response.message = "Package With Code " + package.code + " already Exists.";
                    }
                }
                else
                {
                    response.isSuccess = false;
                    response.status = (int)ResponseStatus.NotFound;
                    response.message = "Invalid Package Code.";
                }
            }
            catch (Exception ex)
            {
                response = new Response(ex);
            }

            return response; ;
        }

        public Response UpdatePackage(Package package)
        {
            Response response;
            try
            {
                response = new Response();
                if (String.IsNullOrEmpty(package.code))
                {
                    Package PackageCode = db.Packages.Where(a => a.code == package.code && a.isActive == true).FirstOrDefault();
                    if (PackageCode == null || package.packageID==PackageCode.packageID)
                    {
                        package.updatedBy = Utilities.GetIdentity(httpContext).userID;
                        package.priceAfterDiscount = package.price - ((package.price * package.offPercent) / 100);
                        package.updateDate = DateTime.Now;

                        Package TEMP = db.Packages.Where(a => a.packageID == package.packageID && a.isActive == true).FirstOrDefault();

                        db.Entry(TEMP).CurrentValues.SetValues(package);
                        db.SaveChanges();
                        response.message = "Package Updated Successfully.";
                    }
                    else
                    {
                        response.isSuccess = false;
                        response.status = (int)ResponseStatus.Conflict;
                        response.message = "Package With Code " + package.code + " already Exists.";
                    }
                }
                else
                {
                    response.isSuccess = false;
                    response.status = (int)ResponseStatus.NotFound;
                    response.message = "Invalid Package Code.";
                }
            }
            catch (Exception ex)
            {
                response = new Response(ex);
            }

            return response; ;
        }

        public Response DeletePackage(Package package)
        {
            Response response;
            try
            {
                response = new Response();
                Package TEMP = db.Packages.Where(a => a.packageID == package.packageID && a.isActive == true).FirstOrDefault();
                TEMP.isActive = false;
                db.Entry(TEMP).CurrentValues.SetValues(TEMP);
                response.message = "Package Updated Successfully.";
            }
            catch (Exception ex)
            {
                response = new Response(ex);
            }

            return response; ;
        }
    }
}
