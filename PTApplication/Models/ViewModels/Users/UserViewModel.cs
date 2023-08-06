using FirebaseAdmin.Messaging;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using PTApplication.Models.DBContext;
using PTApplication.Models.Global;
using PTApplication.Models.ORM;
using PTApplication.Models.ViewModels.RechargeVM;

namespace PTApplication.Models.ViewModels.Users
{
    public class UserViewModel
    {
        private readonly PTAppDBContext db;
        private readonly IConfiguration configuration;
        private readonly HttpContext httpContext;
        public UserViewModel(PTAppDBContext _db, IConfiguration _configuration, HttpContext _httpContext)
        {
            db = _db;
            configuration = _configuration;
            httpContext = _httpContext;
        }

        public Response GetAllActivePTs()
        {
            Response response;
            try
            {
                response = new Response();
                Guid? uID = Utilities.GetIdentity(httpContext).userID;
                var ids = db.RequestReply.Where(x => x.clientID == uID && x.statusID!= (int)RequestReplyStatus.Rejected && x.isActive == true).Select(x => x.pTID).ToArray();

                List<User> userList = db.Users.Where(x => !ids.Contains(x.userID) && x.accountType.ToLower() == "pt" && x.isActive == true && x.isProfileCompleted==true).ToList();
                foreach (User user in userList)
                {
                    user.email = "************";
                    user.password = "************";
                    user.cell = "************";
                    user.userTags = (from ut in db.UserTags
                                     join t in db.Tags on
                                     ut.tagID equals t.tagID into result
                                     from d in result.DefaultIfEmpty()
                                     where ut.userID == user.userID &&
                                     ut.isActive == true
                                     orderby d.name
                                     select new UserTag
                                     {
                                         name = d.name,
                                         description = d.description,
                                         tagID = ut.tagID,
                                         userID = ut.userID,
                                         userTagID = ut.tagID,
                                         isActive = ut.isActive,
                                         createdBy = ut.createdBy,
                                         updateDate = ut.updateDate,
                                         updatedBy = ut.updatedBy,

                                     }).ToList<UserTag>();
                }
                response.responseObject = userList;
            }
            catch (Exception ex)
            {
                response = new Response(ex);
            }
            return response;
        }

        public Response GetAllActiveClients()
        {
            Response response;
            try
            {
                response = new Response();
                Guid? uID = Utilities.GetIdentity(httpContext).userID;
                var request_ids = db.RequestReply.Where(x => x.pTID == uID && x.statusID != (int)RequestReplyStatus.Rejected && x.isActive == true).Select(x => x.clientID).ToArray();

                var ids = db.Conversions.Where(x => x.ptID == uID && x.isActive == true).Select(x => x.clientID).ToArray();
                
                List<User> userList = db.Users.Where(x => !request_ids.Contains(x.userID) && !ids.Contains(x.userID) && x.accountType.ToLower() == "client" && x.isActive == true && x.isProfileCompleted==true).ToList();
                foreach (User user in userList)
                { 
                    user.isDiscountApplicable = true;
                    
                    user.requiredCreditsForContactAfterDiscount = user.requiredCreditsForContact;
                    try
                        {

                        if (db.RequestReply.Any(x => x.clientID == user.userID && x.pTID == uID && x.statusID == (int)RequestReplyStatus.Approved && x.isActive == true))
                        {
                            user.isDiscountApplicable = false;
                        }
                        if (db.Conversions.Count(x => x.clientID == user.userID && x.isActive == true) >= 5)
                        {
                            user.isDiscountApplicable = false;
                        }

                        DateTime startDate = Convert.ToDateTime(user.profileCompleteDate);
                            DateTime endDate = System.DateTime.Now;
                            double days = (endDate - startDate).TotalDays;
                            if (days >= 8 && user.isDiscountApplicable==true)
                            {
                                user.isDiscountApplicable = true;
                                decimal discount = (decimal)((user.requiredCreditsForContact * 30) / 100);
                                user.requiredCreditsForContactAfterDiscount = user.requiredCreditsForContact - discount;
                            }
                            else
                            {
                                user.isDiscountApplicable = false;
                                user.requiredCreditsForContactAfterDiscount = user.requiredCreditsForContact;
                            }
                        }
                        catch (Exception ex)
                        {
                            user.isDiscountApplicable = false;
                        user.requiredCreditsForContactAfterDiscount = user.requiredCreditsForContact;
                    }

                    user.email = "************";
                    user.password = "************";
                    user.cell = "************";
                    user.userTags = (from ut in db.UserTags
                                     join t in db.Tags on
                                     ut.tagID equals t.tagID into result
                                     from d in result.DefaultIfEmpty()
                                     where ut.userID == user.userID &&
                                     ut.isActive == true
                                     orderby d.name
                                     select new UserTag
                                     {
                                         name = d.name,
                                         description = d.description,
                                         tagID = ut.tagID,
                                         userID = ut.userID,
                                         userTagID = ut.tagID,
                                         isActive = ut.isActive,
                                         createdBy = ut.createdBy,
                                         updateDate = ut.updateDate,
                                         updatedBy = ut.updatedBy
                                     }).ToList();
                }
                response.responseObject = userList;
            }
            catch (Exception ex)
            {
                response = new Response(ex);
            }
            return response;
        }


        //
        public Response GetConversions()
        {
            Response response;
            try
            {
                response = new Response();
                List<User> userList = new List<User>();
                Guid? uID = Utilities.GetIdentity(httpContext).userID;
                List<Recharge> conversions = db.Recharge.Where(x => x.userID == uID && x.rechargeStatus.ToLower() == "deduction" && x.isActive == true).ToList();
                foreach (Recharge conversion in conversions)
                {
                    userList.Add(db.Users.Where(x => x.userID == conversion.clientID).FirstOrDefault());
                }
                userList.RemoveAll(x => x == null);
                response.responseObject = userList;
            }
            catch (Exception ex)
            {
                response = new Response(ex);
            }
            return response;
        }
        public int GetCreditsForClientContact(Guid clientID)
        {
            int TotalCredits = 0;
            try
            {

                List<UserBioAnswer> bioAnswers = db.UserBioAnswers.Where(x => x.userID == clientID && x.isActive == true).ToList();
                foreach (var ans in bioAnswers)
                {
                    BioOptions option = db.BioOptions.Where(x => x.optionID == ans.optionID).First();
                    if (option != null)
                    {
                        TotalCredits = TotalCredits + (option.points ?? 0);
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return TotalCredits;
        }


        #region Request Reply

        public Response AddRequestReply(RequestReply requestReply)
        {
            Response response;
            try
            {
                response = new Response();
                Guid? uID = Utilities.GetIdentity(httpContext).userID;
                requestReply.requestReplyID = Guid.NewGuid();
                requestReply.clientID = uID;
                requestReply.creationDate = Convert.ToDateTime(System.DateTime.Now);
                requestReply.createdBy = uID;
                requestReply.status = RequestReplyStatus.ApprovalPending.ToString();
                requestReply.statusID = (int)RequestReplyStatus.ApprovalPending;

                RequestReply previousRequestReply = db.RequestReply.Where(x => x.clientID == requestReply.clientID && x.pTID == requestReply.pTID && x.isActive == true).FirstOrDefault();


                if (previousRequestReply == null || previousRequestReply.statusID == (int)RequestReplyStatus.Rejected)
                {
                    db.RequestReply.Add(requestReply);
                    db.SaveChanges();
                    response.isSuccess = true;
                    response.message = "Your Request For This Trainer Is Submitted Successfully For Approval.";
                    response.summary = "Your Request For This Trainer Is Submitted Successfully For Approval.";
                }
                else
                {
                    if (previousRequestReply.statusID == (int)RequestReplyStatus.Approved)
                    {
                        response.status = (int)ResponseStatus.Forbidden;
                        response.isSuccess = false;
                        response.message = "Your Request For This Trainer Already Approved.";
                        response.summary = "Your Request For This Trainer Already Approved.";
                    }
                    else if (previousRequestReply.statusID == (int)RequestReplyStatus.ApprovalPending)
                    {
                        response.status = (int)ResponseStatus.Forbidden;
                        response.isSuccess = false;
                        response.message = "Your Request For This Trainer Is Already Submitted For Approval.";
                        response.summary = "Your Request For This Trainer Is Already Submitted For Approval.";
                    }
                    else if(previousRequestReply.statusID == (int)RequestReplyStatus.Rejected)
                    {
                        db.RequestReply.Add(requestReply);
                        db.SaveChanges();
                        response.message = "Your Request For This Trainer Is Submitted Successfully For Approval.";
                        response.summary = "Your Request For This Trainer Is Submitted Successfully For Approval.";
                        response.isSuccess = true;
                    }
                    else
                    {
                        response.status = (int)ResponseStatus.BadRequest;
                        response.isSuccess = false;
                        response.message = "Your Request For This Trainer Not Processed Successfully.";
                        response.summary = "Your Request For This Trainer Not Processed Successfully.";
                    }
                }
            }
            catch (Exception ex)
            {
                response = new Response(ex);
            }
            return response;
        }


        public Response GetMyRequestReply()
        {
            Response response;
            try
            {
                response = new Response();
                Guid? uID = Utilities.GetIdentity(httpContext).userID;
                User currentUser = db.Users.Where(x => x.userID == uID).FirstOrDefault();
                if (currentUser != null && currentUser.accountType.ToLower() == "pt")
                {
                    var ids = db.Conversions.Where(x => x.ptID == uID && x.isActive == true).Select(x => x.clientID).ToArray();

                    List<RequestReplyReport> userList = (from r in db.RequestReply
                                    join u in db.Users on
                                    r.clientID equals u.userID into result
                                    from d in result.DefaultIfEmpty()
                                    where (!ids.Contains(r.clientID)) &&
                                    r.isActive == true && r.pTID==uID
                                    orderby r.creationDate
                                    select new RequestReplyReport
                                    {
                                        requestReplyID = r.requestReplyID,
                                        pTID = r.pTID,
                                        clientID = r.clientID,
                                        status = r.status,
                                        statusID = r.statusID,
                                        requestMessage = r.requestMessage,
                                        ptComments = r.ptComments,
                                        creationDate = r.creationDate,
                                        userID = d.userID,
                                        firstName = d.firstName,
                                        lastName = d.lastName,
                                        companyPhone = d.companyPhone,
                                        companyName = d.companyName,
                                        companyLocation = d.companyLocation,
                                        accountType = d.accountType,
                                        cell = d.cell,
                                        companyDescription = d.companyDescription,
                                        companyEmail = d.companyEmail,
                                        companySize = d.companySize,
                                        email = d.email,
                                        frequencyTime = d.frequencyTime,
                                        fullName = d.fullName,
                                        latitude = d.latitude,
                                        longitude = d.longitude,
                                        locationShownOnProfile = d.locationShownOnProfile,
                                        postCode = d.postCode,
                                        postedTime = d.postedTime,
                                        profilePicture = d.profilePicture,
                                        radius = d.radius,
                                        region = d.region,
                                        requiredCreditsForContact = d.requiredCreditsForContact,
                                        serviceLocation1 = d.serviceLocation1,
                                        serviceLocation2 = d.serviceLocation2,
                                        surName = d.surName,
                                        website = d.website,
                                        username = d.username,
                                        yearsInBusiness = d.yearsInBusiness,
                                        userTags = d.userTags
                                    }).ToList();


                    //List<User> userList = db.Users.Where(x => ids.Contains(x.userID) && x.isActive == true).ToList();
                    foreach (RequestReplyReport user in userList)
                    {
                        user.userTags = (from ut in db.UserTags
                                         join t in db.Tags on
                                         ut.tagID equals t.tagID into result
                                         from d in result.DefaultIfEmpty()
                                         where ut.userID == user.userID &&
                                         ut.isActive == true
                                         orderby d.name
                                         select new UserTag
                                         {
                                             name = d.name,
                                             description = d.description,
                                             tagID = ut.tagID,
                                             userID = ut.userID,
                                             userTagID = ut.tagID,
                                             isActive = ut.isActive,
                                             createdBy = ut.createdBy,
                                             updateDate = ut.updateDate,
                                             updatedBy = ut.updatedBy
                                         }).ToList();
                    }
                    response.responseObject = userList;
                }
                else
                {
                    var ids = db.Conversions.Where(x => x.clientID == uID && x.isActive == true).Select(x => x.ptID).ToArray();

                    List<RequestReplyReport> userList =(from r in db.RequestReply
                     join u in db.Users on
                     r.pTID equals u.userID into result
                     from d in result.DefaultIfEmpty()
                     where //ids.Contains(r.pTID) &&
                     r.isActive == true && r.clientID==uID
                     orderby r.creationDate
                     select new RequestReplyReport
                     {
                         requestReplyID = r.requestReplyID,
                         pTID = r.pTID,
                         clientID = r.clientID,
                         status = r.status,
                         statusID = r.statusID,
                         requestMessage = r.requestMessage,
                         ptComments = r.ptComments,
                         creationDate = r.creationDate,
                         userID = d.userID,
                         firstName = d.firstName,
                         lastName = d.lastName,
                         companyPhone = d.companyPhone,
                         companyName = d.companyName,
                         companyLocation = d.companyLocation,
                         accountType = d.accountType,
                         cell = d.cell,
                         companyDescription = d.companyDescription,
                         companyEmail = d.companyEmail,
                         companySize = d.companySize,
                         email = d.email,
                         frequencyTime = d.frequencyTime,
                         fullName = d.fullName,
                         latitude = d.latitude,
                         longitude = d.longitude,
                         locationShownOnProfile = d.locationShownOnProfile,
                         postCode = d.postCode,
                         postedTime = d.postedTime,
                         profilePicture = d.profilePicture,
                         radius = d.radius,
                         region = d.region,
                         requiredCreditsForContact = d.requiredCreditsForContact,
                         serviceLocation1 = d.serviceLocation1,
                         serviceLocation2 = d.serviceLocation2,
                         surName = d.surName,
                         website = d.website,
                         username = d.username,
                         yearsInBusiness = d.yearsInBusiness,
                         userTags= d.userTags
                     }).ToList();

                    //List<User> userList = db.Users.Where(x => ids.Contains(x.userID) && x.isActive == true).ToList();
                    foreach (RequestReplyReport user in userList)
                    {
                        user.userTags = (from ut in db.UserTags
                                         join t in db.Tags on
                                         ut.tagID equals t.tagID into result
                                         from d in result.DefaultIfEmpty()
                                         where ut.userID == user.userID &&
                                         ut.isActive == true
                                         orderby d.name
                                         select new UserTag
                                         {
                                             name = d.name,
                                             description = d.description,
                                             tagID = ut.tagID,
                                             userID = ut.userID,
                                             userTagID = ut.tagID,
                                             isActive = ut.isActive,
                                             createdBy = ut.createdBy,
                                             updateDate = ut.updateDate,
                                             updatedBy = ut.updatedBy
                                         }).ToList();
                    }
                    response.responseObject = userList;
                }
            }
            catch (Exception ex)
            {
                response = new Response(ex);
            }
            return response;
        }
        public Response GetMyRequestReplyForApproval()
        {
            Response response;
            try
            {
                
                response = new Response();
                Guid? uID = Utilities.GetIdentity(httpContext).userID;
                User currentUser = db.Users.Where(x => x.userID == uID).FirstOrDefault();
                if (currentUser != null && currentUser.accountType.ToLower() == "pt")
                {
                    var ids = db.Conversions.Where(x => x.ptID == uID && x.isActive == true).Select(x => x.clientID).ToArray();

                    List<RequestReplyReport> userList = (from r in db.RequestReply
                                                         join u in db.Users on
                                                         r.clientID equals u.userID into result
                                                         from d in result.DefaultIfEmpty()
                                                         where (!ids.Contains(r.clientID)) &&
                                                         r.isActive == true && r.pTID == uID && r.statusID == (int)RequestReplyStatus.ApprovalPending
                                                         orderby r.creationDate
                                                         select new RequestReplyReport
                                                         {
                                                             requestReplyID = r.requestReplyID,
                                                             pTID = r.pTID,
                                                             clientID = r.clientID,
                                                             status = r.status,
                                                             statusID = r.statusID,
                                                             requestMessage = r.requestMessage,
                                                             ptComments = r.ptComments,
                                                             creationDate = r.creationDate,
                                                             userID = d.userID,
                                                             firstName = d.firstName,
                                                             lastName = d.lastName,
                                                             companyPhone = d.companyPhone,
                                                             companyName = d.companyName,
                                                             companyLocation = d.companyLocation,
                                                             accountType = d.accountType,
                                                             cell = d.cell,
                                                             companyDescription = d.companyDescription,
                                                             companyEmail = d.companyEmail,
                                                             companySize = d.companySize,
                                                             email = d.email,
                                                             frequencyTime = d.frequencyTime,
                                                             fullName = d.fullName,
                                                             latitude = d.latitude,
                                                             longitude = d.longitude,
                                                             locationShownOnProfile = d.locationShownOnProfile,
                                                             postCode = d.postCode,
                                                             postedTime = d.postedTime,
                                                             profilePicture = d.profilePicture,
                                                             radius = d.radius,
                                                             region = d.region,
                                                             requiredCreditsForContact = d.requiredCreditsForContact,
                                                             serviceLocation1 = d.serviceLocation1,
                                                             serviceLocation2 = d.serviceLocation2,
                                                             surName = d.surName,
                                                             website = d.website,
                                                             username = d.username,
                                                             yearsInBusiness = d.yearsInBusiness,
                                                             userTags = d.userTags
                                                         }).ToList();


                    //List<User> userList = db.Users.Where(x => ids.Contains(x.userID) && x.isActive == true).ToList();
                    foreach (RequestReplyReport user in userList)
                    {
                        user.userTags = (from ut in db.UserTags
                                         join t in db.Tags on
                                         ut.tagID equals t.tagID into result
                                         from d in result.DefaultIfEmpty()
                                         where ut.userID == user.userID &&
                                         ut.isActive == true
                                         orderby d.name
                                         select new UserTag
                                         {
                                             name = d.name,
                                             description = d.description,
                                             tagID = ut.tagID,
                                             userID = ut.userID,
                                             userTagID = ut.tagID,
                                             isActive = ut.isActive,
                                             createdBy = ut.createdBy,
                                             updateDate = ut.updateDate,
                                             updatedBy = ut.updatedBy
                                         }).ToList();
                    }
                    response.responseObject = userList;
                }
                else
                {
                    var ids = db.Conversions.Where(x => x.clientID == uID && x.isActive == true).Select(x => x.ptID).ToArray();

                    List<RequestReplyReport> userList = (from r in db.RequestReply
                                                         join u in db.Users on
                                                         r.pTID equals u.userID into result
                                                         from d in result.DefaultIfEmpty()
                                                         where //ids.Contains(r.pTID) &&
                                                         r.isActive == true && r.clientID == uID && r.statusID == (int)RequestReplyStatus.ApprovalPending
                                                         orderby r.creationDate
                                                         select new RequestReplyReport
                                                         {
                                                             requestReplyID = r.requestReplyID,
                                                             pTID = r.pTID,
                                                             clientID = r.clientID,
                                                             status = r.status,
                                                             statusID = r.statusID,
                                                             requestMessage = r.requestMessage,
                                                             ptComments = r.ptComments,
                                                             creationDate = r.creationDate,
                                                             userID = d.userID,
                                                             firstName = d.firstName,
                                                             lastName = d.lastName,
                                                             companyPhone = d.companyPhone,
                                                             companyName = d.companyName,
                                                             companyLocation = d.companyLocation,
                                                             accountType = d.accountType,
                                                             cell = d.cell,
                                                             companyDescription = d.companyDescription,
                                                             companyEmail = d.companyEmail,
                                                             companySize = d.companySize,
                                                             email = d.email,
                                                             frequencyTime = d.frequencyTime,
                                                             fullName = d.fullName,
                                                             latitude = d.latitude,
                                                             longitude = d.longitude,
                                                             locationShownOnProfile = d.locationShownOnProfile,
                                                             postCode = d.postCode,
                                                             postedTime = d.postedTime,
                                                             profilePicture = d.profilePicture,
                                                             radius = d.radius,
                                                             region = d.region,
                                                             requiredCreditsForContact = d.requiredCreditsForContact,
                                                             serviceLocation1 = d.serviceLocation1,
                                                             serviceLocation2 = d.serviceLocation2,
                                                             surName = d.surName,
                                                             website = d.website,
                                                             username = d.username,
                                                             yearsInBusiness = d.yearsInBusiness,
                                                             userTags = d.userTags
                                                         }).ToList();

                    //List<User> userList = db.Users.Where(x => ids.Contains(x.userID) && x.isActive == true).ToList();
                    foreach (RequestReplyReport user in userList)
                    {
                        user.userTags = (from ut in db.UserTags
                                         join t in db.Tags on
                                         ut.tagID equals t.tagID into result
                                         from d in result.DefaultIfEmpty()
                                         where ut.userID == user.userID &&
                                         ut.isActive == true
                                         orderby d.name
                                         select new UserTag
                                         {
                                             name = d.name,
                                             description = d.description,
                                             tagID = ut.tagID,
                                             userID = ut.userID,
                                             userTagID = ut.tagID,
                                             isActive = ut.isActive,
                                             createdBy = ut.createdBy,
                                             updateDate = ut.updateDate,
                                             updatedBy = ut.updatedBy
                                         }).ToList();
                    }
                    response.responseObject = userList;
                }
            }
            catch (Exception ex)
            {
                response = new Response(ex);
            }
            return response;
        }

        public Response ApproveRequestReply(RequestReply requestReply)
        {
            Response response;
            try
            {
                response = new Response();
                Guid? uID = Utilities.GetIdentity(httpContext).userID;

                RequestReply myRequest = db.RequestReply.Where(x => x.requestReplyID == requestReply.requestReplyID && x.pTID==uID && x.statusID == (int)RequestReplyStatus.ApprovalPending && x.isActive == true).FirstOrDefault();
                if (myRequest != null)
                {
                    myRequest.status = RequestReplyStatus.Approved.ToString();
                    myRequest.statusID = (int)RequestReplyStatus.Approved;
                    myRequest.ptComments = requestReply.ptComments;
                    db.Entry(myRequest).CurrentValues.SetValues(myRequest);
                    Recharge recharge = new Recharge();
                    recharge.clientID = myRequest.clientID;
                    Response rechargeResponse= new RechargeViewModel(db,configuration, httpContext).DeductBalance(recharge);
                    if (!rechargeResponse.isSuccess)
                    {
                        throw new Exception(rechargeResponse.message);
                    }

                    User requestor= db.Users.Where(x => x.userID == myRequest.clientID).FirstOrDefault();
                    if(requestor!=null)
                    {
                        requestor.isDiscountApplicable = false;
                        db.Entry(requestor).CurrentValues.SetValues(requestor);
                        db.SaveChanges();
                    }
                    db.SaveChanges();
                    response.message = "Request Approved Successfully.";
                    response.summary = "Request Approved Successfully.";

                }
                else
                {
                    response.status = (int)ResponseStatus.NotFound;
                    response.isSuccess = false;
                    response.message = "Client Request Not Found.";
                    response.summary = "Client Request Not Found.";
                }



            }
            catch (Exception ex)
            {
                response = new Response(ex);
            }
            return response;
        }
        public Response RejectRequestReply(RequestReply requestReply)
        {
            Response response;
            try
            {
                response = new Response();
                Guid? uID = Utilities.GetIdentity(httpContext).userID;

                RequestReply myRequest = db.RequestReply.Where(x => x.requestReplyID == requestReply.requestReplyID && x.pTID == uID && x.statusID == (int)RequestReplyStatus.ApprovalPending && x.isActive == true).FirstOrDefault();
                if (myRequest != null)
                {
                    myRequest.status = RequestReplyStatus.Rejected.ToString();
                    myRequest.statusID = (int)RequestReplyStatus.Rejected;
                    myRequest.ptComments = requestReply.ptComments;
                    db.Entry(myRequest).CurrentValues.SetValues(myRequest);
                    db.SaveChanges();
                    response.message = "Request Rejected Successfully.";
                    response.summary = "Request Rejected Successfully.";

                }
                else
                {
                    response.status = (int)ResponseStatus.NotFound;
                    response.isSuccess = false;
                    response.message = "Client Request Not Found.";
                    response.summary = "Client Request Not Found.";
                }



            }
            catch (Exception ex)
            {
                response = new Response(ex);
            }
            return response;
        }

        

        #endregion
    }
}
