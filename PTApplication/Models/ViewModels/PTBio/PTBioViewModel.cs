using PTApplication.Models.DBContext;
using PTApplication.Models.Global;
using PTApplication.Models.ORM;

namespace PTApplication.Models.ViewModels.PTBio
{
    public class PTBioViewModel
    {
        private readonly PTAppDBContext db;
        private readonly IConfiguration configuration;
        private readonly HttpContext httpContext;
        public PTBioViewModel(PTAppDBContext _db, IConfiguration _configuration, HttpContext _httpContext)
        {
            db = _db;
            configuration = _configuration;
            httpContext = _httpContext;
        }

        #region Questions
        public Response GetQuestionByKey(Guid questionID)
        {
            Response response;
            try
            {
                response = new Response();
                BioQuestion bioQuestions = db.BioQuestions.Where(a => a.questionID == questionID && a.isActive == true).FirstOrDefault();
                response.responseObject = bioQuestions;
            }
            catch (Exception ex)
            {
                response = new Response(ex);
            }
            return response;
        }
        public Response GetAllQuestion()
        {
            Response response;
            try
            {
                response = new Response();
                List<BioQuestion> bioQuestions = db.BioQuestions.Where(a => a.isActive == true).OrderBy(o => o.serialNo).ToList();
                response.responseObject = bioQuestions;
            }
            catch (Exception ex)
            {
                response = new Response(ex);
            }
            return response;
        }
        public Response AddQuestion(BioQuestion bioQuestion)
        {
            Response response;
            try
            {
                response = new Response();
                bioQuestion.questionID = Guid.NewGuid();
                db.BioQuestions.Add(bioQuestion);
                db.SaveChanges();
                response.message = "Bio Question Added Successfully.";
            }
            catch (Exception ex)
            {
                response = new Response(ex);
            }

            return response; ;
        }

        public Response UpdateQuestion(BioQuestion bioQuestion)
        {
            Response response;
            try
            {
                response = new Response();
                BioQuestion TEMP = db.BioQuestions.Where(a => a.questionID == bioQuestion.questionID && a.isActive == true).FirstOrDefault();
                db.Entry(TEMP).CurrentValues.SetValues(bioQuestion);
                db.SaveChanges ();
                response.message = "Bio Question Updated Successfully.";
            }
            catch (Exception ex)
            {
                response = new Response(ex);
            }

            return response; ;
        }

        public Response DeleteQuestion(BioQuestion bioQuestion)
        {
            Response response;
            try
            {
                response = new Response();
                BioQuestion TEMP = db.BioQuestions.Where(a => a.questionID == bioQuestion.questionID && a.isActive == true).FirstOrDefault();
                TEMP.isActive = false;
                db.Entry(TEMP).CurrentValues.SetValues(TEMP);
                db.SaveChanges();
                response.message = "Bio Question Deleted Successfully.";
            }
            catch (Exception ex)
            {
                response = new Response(ex);
            }

            return response; ;
        }
        #endregion

        #region Options
        public Response GetOptionByKey(Guid optionID)
        {
            Response response;
            try
            {
                response = new Response();
                BioOptions bioOptions = db.BioOptions.Where(a => a.optionID == optionID && a.isActive == true).FirstOrDefault();
                response.responseObject = bioOptions;
            }
            catch (Exception ex)
            {
                response = new Response(ex);
            }
            return response;
        }
        public Response GetOptionByQusetionID(Guid questionID)
        {
            Response response;
            try
            {
                response = new Response();
                List<BioOptions> bioOptions = db.BioOptions.Where(a => a.questionID == questionID && a.isActive == true).ToList();
                response.responseObject = bioOptions;
            }
            catch (Exception ex)
            {
                response = new Response(ex);
            }
            return response;
        }
        public Response GetAllOptions()
        {
            Response response;
            try
            {
                response = new Response();
                List<BioOptions> bioOptions = db.BioOptions.Where(a => a.isActive == true).OrderBy(o => o.serialNo).ToList();
                response.responseObject = bioOptions;
            }
            catch (Exception ex)
            {
                response = new Response(ex);
            }
            return response;
        }

        public Response AddQuestionOption(BioOptions bioOptions)
        {
            Response response;
            try
            {
                response = new Response();
                bioOptions.optionID = Guid.NewGuid();
                db.BioOptions.Add(bioOptions);
                db.SaveChanges();
                response.message = "Bio Question Option Added Successfully.";
            }
            catch (Exception ex)
            {
                response = new Response(ex);
            }

            return response; ;
        }

        public Response UpdateQuestionOption(BioOptions bioOptions)
        {
            Response response;
            try
            {
                response = new Response();
                BioOptions TEMP = db.BioOptions.Where(a => a.optionID == bioOptions.optionID && a.isActive == true).FirstOrDefault();
                db.Entry(TEMP).CurrentValues.SetValues(bioOptions);
                db.SaveChanges();
                response.message = "Bio Question Option Updated Successfully.";
            }
            catch (Exception ex)
            {
                response = new Response(ex);
            }

            return response; ;
        }

        public Response DeleteQuestionOption(BioOptions bioOptions)
        {
            Response response;
            try
            {
                response = new Response();
                BioOptions TEMP = db.BioOptions.Where(a => a.optionID == bioOptions.optionID && a.isActive == true).FirstOrDefault();
                TEMP.isActive = false;
                db.Entry(TEMP).CurrentValues.SetValues(TEMP);
                db.SaveChanges();
                response.message = "Bio Question Option Deleted Successfully.";
            }
            catch (Exception ex)
            {
                response = new Response(ex);
            }

            return response; ;
        }
        #endregion

        #region General
        public Response GetAllQuestionWithOptions()
        {
            Response response;
            try
            {
                Guid? UID = Utilities.GetIdentity(httpContext).userID;
                List<UserBioAnswer> userBioAnswer = db.UserBioAnswers.Where(a => a.userID == UID && a.isActive==true).ToList();
                response = new Response();
                List<BioQuestion> questionList = db.BioQuestions.OrderBy(o => o.serialNo).ToList();
                foreach (var question in questionList)
                {
                    List<UserBioAnswer> TempQuestionOptions = userBioAnswer.Where(x => x.questionID == question.questionID).ToList();
                    question.bioOptions = db.BioOptions.Where(x => x.questionID == question.questionID).OrderBy(o=>o.serialNo).ToList();
                    foreach (var o in TempQuestionOptions)
                    {
                        question.bioOptions.Where(x=>x.optionID== o.optionID && x.questionID==question.questionID).FirstOrDefault().isChecked = true;
                    }
                }
                response.responseObject = questionList;
            }
            catch (Exception ex)
            {
                response = new Response(ex);
            }
            return response;
        }

        public Response AddBioQuestion(BioQuestion bioQuestion)
        {
            Response response;
            try
            {
                response = new Response();
                bioQuestion.questionID=Guid.NewGuid();
                db.BioQuestions.Add(bioQuestion);
                db.SaveChanges();
                response.message = "Bio Question Added Successfully.";
            }
            catch (Exception ex)
            {
                response=new Response(ex);
            }

            return response; ;
        }

        public Response AddBioOptions(BioOptions bioOptions)
        {
            Response response;
            try
            {
                response = new Response();
                bioOptions.optionID = Guid.NewGuid();
                db.BioOptions.Add(bioOptions);
                db.SaveChanges();
                response.message = "Bio Question Option Added Successfully.";
            }
            catch (Exception ex)
            {
                response = new Response(ex);
            }

            return response; ;
        }


        #endregion
    }
}
