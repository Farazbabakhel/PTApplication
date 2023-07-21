using PTApplication.Models.DBContext;
using PTApplication.Models.Global;
using PTApplication.Models.ORM;

namespace PTApplication.Models.ViewModels.Setting
{
    public class PTSettingViewModel
    {
        private readonly PTAppDBContext db;
        private readonly IConfiguration configuration;
        private readonly HttpContext httpContext;
        public PTSettingViewModel(PTAppDBContext _db, IConfiguration _configuration, HttpContext _httpContext)
        {
            db = _db;
            configuration = _configuration;
            httpContext = _httpContext;
        }

        public Response GetMySettings()
        {
            Response response;
            try
            {
                Guid? uID = Utilities.GetIdentity(httpContext).userID;
                response = new Response();
                PTSetting pTSetting = db.PTSettings.Where(a => a.userID == uID && a.isActive == true).FirstOrDefault();
                response.responseObject = pTSetting;
            }
            catch (Exception ex)
            {
                response = new Response(ex);
            }
            return response;
        }
        public Response GetUserSettings(Guid userID)
        {
            Response response;
            try
            {
                response = new Response();
                PTSetting pTSetting = db.PTSettings.Where(a => a.userID == userID && a.isActive == true).FirstOrDefault();
                response.responseObject = pTSetting;
            }
            catch (Exception ex)
            {
                response = new Response(ex);
            }
            return response;
        }

        public Response AddUpdate(PTSetting setting)
        {
            Response response;
            try
            {
                Guid? userID = Utilities.GetIdentity(httpContext).userID;
                response = new Response();
                PTSetting pTSetting = db.PTSettings.Where(a => a.userID == userID && a.isActive == true).FirstOrDefault(); ;
                if (pTSetting == null)
                {
                    setting.pTSettingID = Guid.NewGuid();
                    setting.createdBy = userID;
                    setting.userID = userID;
                    setting.creationDate = Convert.ToDateTime(System.DateTime.Now);
                    db.PTSettings.Add(setting);
                    db.SaveChanges();
                    response.responseObject = setting;
                    response.heading = "Settings Added!";
                    response.message = "PT Settings Added Successfully.";
                    response.summary = "PT Settings Added Successfully.";
                }
                else
                {
                    setting.pTSettingID = pTSetting.pTSettingID;
                    setting.updatedBy = userID;
                    setting.userID = pTSetting.userID;
                    setting.createdBy = pTSetting.createdBy;
                    setting.creationDate = pTSetting.creationDate;
                    setting.updateDate = Convert.ToDateTime(System.DateTime.Now);
                    PTSetting Temp=db.PTSettings.Where(x=>x.pTSettingID==pTSetting.pTSettingID).FirstOrDefault();
                    db.Entry(Temp).CurrentValues.SetValues(setting);
                    db.SaveChanges();
                    response.heading = "Settings Updated!";
                    response.message = "PT Settings Updated Successfully.";
                    response.summary = "PT Settings Updated Successfully.";
                }
            }
            catch (Exception ex)
            {
                response = new Response(ex);
            }
            return response;
        }


    }
}
