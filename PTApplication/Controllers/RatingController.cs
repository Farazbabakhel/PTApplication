using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PTApplication.Models.DBContext;
using PTApplication.Models.Global;
using PTApplication.Models.ORM;
using PTApplication.Models.ViewModels.RechargeVM;
using System;

namespace PTApplication.Controllers
{ 
    [Route("PTAPI/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class RatingController : ControllerBase
    {
        private readonly PTAppDBContext db;
        public IConfiguration configuration { get; }
        public RatingController(PTAppDBContext _db, IConfiguration _config)
        {
            db = _db;
            configuration = _config;
        }

        [HttpGet]
        public Response ReviewForMe()
        {
            Response response;
            try
            {
                response = new Response();
                Guid UserID=(Guid)Utilities.GetIdentity(HttpContext).userID;
                response.responseObject = db.Ratings.Where(x => x.ratingForUserID == UserID).ToList();
            }
            catch (Exception ex)
            {
                response = new Response(ex);
            }
            return response;
        }

        [HttpGet]
        public Response ReviewForUserID(Guid userID)
        {
            Response response;
            try
            {
                response = new Response();
                response.responseObject = db.Ratings.Where(x => x.ratingForUserID == userID).ToList();
            }
            catch (Exception ex)
            {
                response = new Response(ex);
            }
            return response;
        }

        [HttpGet]
        public Response ReviewByMe()
        {
            Response response;
            try
            {
                response = new Response();
                Guid UserID = (Guid)Utilities.GetIdentity(HttpContext).userID;
                response.responseObject = db.Ratings.Where(x => x.ratingByUserID == UserID).ToList();
            }
            catch (Exception ex)
            {
                response = new Response(ex);
            }
            return response;
        }
        [HttpGet]
        public Response ReviewByUserID(Guid userID)
        {
            Response response;
            try
            {
                response = new Response();
                response.responseObject = db.Ratings.Where(x => x.ratingByUserID == userID).ToList();
            }
            catch (Exception ex)
            {
                response = new Response(ex);
            }
            return response;
        }


        [HttpPost]
        public Response Add([FromBody] Rating rating)
        {
            Response response;
            try
            {
                response = new Response();
                Guid UserID = (Guid)Utilities.GetIdentity(HttpContext).userID;
                Rating Temp = db.Ratings.Where(x=>x.ratingByUserID== UserID && x.ratingForUserID==rating.ratingForUserID && x.isActive==true).FirstOrDefault();
                if (Temp == null)
                {
                    rating.ratingID = Guid.NewGuid();
                    rating.ratingByUserID = UserID;
                    rating.creationDate = Convert.ToDateTime(System.DateTime.Now);
                    rating.totalRatingCount = 5;
                    db.Ratings.Add(rating);
                    db.SaveChanges();
                    response.heading = "Added";
                    response.message = "Rating Added Successfully";
                    response.summary = "Rating Added Successfully";
                }
                else
                {
                    if(!String.IsNullOrEmpty(rating.comments))
                    {
                        Temp.comments = rating.comments;
                    }
                    
                    Temp.ratingCount = rating.ratingCount;
                    Temp.updatedBy = UserID;
                    Temp.updateDate= Convert.ToDateTime(System.DateTime.Now);
                    db.Entry(Temp).CurrentValues.SetValues(Temp);
                    db.SaveChanges();
                    response.heading = "Updated";
                    response.message = "Rating Updated Successfully";
                    response.summary = "Rating Updated Successfully";
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
