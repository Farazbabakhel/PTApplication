using PTApplication.Models.DBContext;
using PTApplication.Models.Global;
using PTApplication.Models.ORM;
using PTApplication.Models.ViewModels.Users;

namespace PTApplication.Models.ViewModels.Tags
{
    public class TagsViewModel
    {
        private readonly PTAppDBContext db;
        private readonly IConfiguration configuration;
        private readonly HttpContext httpContext;
        public TagsViewModel(PTAppDBContext _db, IConfiguration _configuration, HttpContext _httpContext)
        {
            db = _db;
            configuration = _configuration;
            httpContext = _httpContext;
        }

        public Response GetAllActiveTags()
        {
            Response response;
            try
            {
                response = new Response();
                List<Tag> tags = db.Tags.Where(x=>x.isActive==true).ToList();
                response.responseObject = tags;
            }
            catch (Exception ex)
            {
                response = new Response(ex);
            }
            return response;
        }

        public Response GetUserTags(Guid? userID)
        {
            Response response;
            try
            {
                response = new Response();

                List<UserTag> userTags =  (from ut in db.UserTags
                                          join t in db.Tags on
                                          ut.tagID equals t.tagID into result
                                          from d in result.DefaultIfEmpty()
                                          where ut.userID == userID &&
                                          ut.isActive == true
                                          orderby d.name
                                          select new UserTag
                                          {
                                              name = d.name,
                                              description = d.description,
                                              tagID = ut.tagID,
                                              userID=ut.userID,
                                              userTagID=ut.tagID,
                                              isActive=ut.isActive,
                                              createdBy = ut.createdBy,
                                              updateDate = ut.updateDate,
                                              updatedBy= ut.updatedBy

                                          }).ToList();
                response.responseObject = userTags;



            }
            catch (Exception ex)
            {
                response = new Response(ex);
            }
            return response;
        }
        public Response GetTagByID(Guid? tagID)
        {
            Response response;
            try
            {
                response = new Response();

                response.responseObject = db.Tags.Where(x => x.tagID == tagID).FirstOrDefault();
            }
            catch (Exception ex)
            {
                response = new Response(ex);
            }
            return response;
        }
        public Response GetTagByName(string name)
        {
            Response response;
            try
            {
                response = new Response();

                response.responseObject = db.Tags.Where(x => x.name == name).FirstOrDefault();
                
            }
            catch (Exception ex)
            {
                response = new Response(ex);
            }
            return response;
        }
        public Response AddTags(Tag tag)
        {
            Response response;
            try
            { 
                response = new Response();
                if(GetTagByName(tag.name).responseObject==null)
                {
                   
                    tag.tagID = Guid.NewGuid();
                    //tag.createdBy = Utilities.GetIdentity(httpContext).userID;
                    tag.creationDate=Convert.ToDateTime(DateTime.Now);
                    db.Tags.Add(tag);
                    db.SaveChanges();
                    response.isSuccess = true;
                    response.message = "Tag " + tag.name + " Added successfully";
                    response.summary = "Tag " + tag.name + " Added successfully";
                }
                else
                {
                    response.isSuccess = false;
                    response.message = "Tag " + tag.name + " Already Exixts.";
                    response.summary = "Tag " + tag.name + " Added successfully";
                }
                

            }
            catch (Exception ex)
            {
                response = new Response(ex);
            }
            return response;
        }

        public Response AddUserTags(User user)
        {
            Response response;
            try
            {
                response = new Response();
                Guid? CurrentUserID= Utilities.GetIdentity(httpContext).userID;
                //List<UserTag> userTags = new List<UserTag>();
                //userTags = (List<UserTag>)( GetUserTags(CurrentUserID).responseObject);
                
                // Remove All tags of current user
                db.UserTags.RemoveRange(db.UserTags.Where(x => x.userID == CurrentUserID));
                db.SaveChanges();

                // Insert new selected tags of current user
                foreach (var t in user.userTags)
                { 
                   // if (!userTags.Any(x=>x.userID==t.userID && x.tagID==t.tagID && x.isActive==true))
                   // {
                        t.userTagID = Guid.NewGuid();
                        t.userID = CurrentUserID;
                        db.UserTags.Add(t);
                   // }
                }
                db.SaveChanges();
                response.message ="User Tags Added Successfully.";
                response.summary = "User Tags Added Successfully.";
            }
            catch (Exception ex)
            {
                response=new Response(ex);
            }
            return response;
        }


        public Response AddUserBioAnswer(List<BioQuestion> userAnswer)
        {
            Response response;
            try
            {
                Guid? CurrentUserID = Utilities.GetIdentity(httpContext).userID;
                foreach (var q in userAnswer)
                {
                    if (!(q.bioOptions.Any(x => x.isChecked == true)))
                    {
                        throw new Exception("Please Answer Correctly. All Questions Are Compulsory.");
                    }
                }

                db.UserBioAnswers.RemoveRange(db.UserBioAnswers.Where(x => x.userID == CurrentUserID));
                response = new Response();
                
                foreach (var q in userAnswer) {
                    foreach (var o in q.bioOptions)
                    {
                        if(o.isChecked==true)
                        {
                            
                            UserBioAnswer Temp = new UserBioAnswer();
                            Temp.userID = CurrentUserID;
                            Temp.userBioAnswerID = Guid.NewGuid();
                            Temp.questionID = q.questionID;
                            Temp.optionID = o.optionID;
                            Temp.answerDescription = o.description;
                            Temp.serialNo = o.serialNo;
                            Temp.optionSerialNo = o.serialNo;
                            Temp.questionSerialNo = q.serialNo;
                            Temp.creationDate =Convert.ToDateTime(System.DateTime.Now);
                            Temp.createdBy = CurrentUserID;
                            db.UserBioAnswers.Add(Temp);
                        }
                        

                    }
                }
                db.SaveChanges();

                User user=db.Users.Where(x => x.userID == CurrentUserID).FirstOrDefault();
                if (user != null)
                {
                    user.isBioCompleted = true;
                    user.requiredCreditsForContact = new UserViewModel(db,configuration,httpContext).GetCreditsForClientContact((Guid)CurrentUserID);
                    db.Entry(user).CurrentValues.SetValues(user);
                    db.SaveChanges();
                }
                response.message = "User Tags Added Successfully.";
                response.summary = "User Tags Added Successfully.";
            }
            catch (Exception ex)
            {
                response = new Response(ex);
            }
            return response;
        }
        
    }
}
