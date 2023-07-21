using PTApplication.Models.DBContext;
using PTApplication.Models.Global;
using PTApplication.Models.ORM;
using System.Net.Mail;

using PTApplication.Models.Interface;
using Org.BouncyCastle.Asn1.Ocsp;
using PTApplication.Models.ViewModels.Tags;
using Microsoft.IdentityModel.Tokens;

namespace PTApplication.Models.ViewModels.Users
{
    public class SignupViewModel
    {
        private readonly PTAppDBContext db;
        private readonly IConfiguration configuration;
        //private readonly IMailService mailService;

        private readonly HttpContext httpContext;
        public SignupViewModel(PTAppDBContext _db, IConfiguration _configuration, HttpContext _httpContext) //, IMailService _mailService
        {
            db = _db;
            configuration = _configuration;
            httpContext = _httpContext;
            //mailService = _mailService;
        }

        public Response GetUserByEmail(string email)
        {
            Response response;
            try
            {
                response = new Response();
                response.responseObject = db.Users.Where(x => x.email == email).FirstOrDefault();
            }
            catch (Exception ex)
            {
                response = new Response(ex);
            }
            return response;
        }

        public Response GetUserByUserID()
        {
            Response response;
            try
            {
                Guid? userID = Utilities.GetIdentity(httpContext).userID;
                response = new Response();
                User user = db.Users.Where(x => x.userID == userID).FirstOrDefault();
                if (user != null)
                {
                    var userTags = new TagsViewModel(db, configuration, httpContext).GetUserTags(userID).responseObject;
                    if (response != null)
                    {
                        try
                        {
                            user.userTags = (List<UserTag>)userTags;
                        }
                        catch (Exception ex)
                        {
                            // Do Nothing Here
                        }

                    }

                }
                response.responseObject = user;
            }
            catch (Exception ex)
            {
                response = new Response(ex);
            }
            return response;
        }
        public Response GetUserProfileByUserID(Guid userID)
        {
            Response response;
            try
            {
                response = new Response();
                User user = db.Users.Where(x => x.userID == userID).FirstOrDefault();
                if (user != null)
                {
                    var userTags = new TagsViewModel(db, configuration, httpContext).GetUserTags(userID).responseObject;
                    if (user != null)
                    {
                        try
                        {
                            user.userTags = (List<UserTag>)userTags;
                        }
                        catch (Exception ex)
                        {
                            // Do Nothing Here
                        }

                    }

                    var userRating = (from r in db.Ratings
                                      join u in db.Users on
                                      r.ratingByUserID equals u.userID into result
                                      from d in result.DefaultIfEmpty()
                                      where r.ratingForUserID == userID &&
                                      r.isActive == true
                                      orderby r.creationDate
                                      select new Rating
                                      {
                                          userName = d.fullName,
                                          description = r.description,
                                          comments = r.comments,
                                          ratingID = r.ratingID,
                                          ratingByUserID = r.ratingByUserID,
                                          ratingForUserID = r.ratingForUserID,
                                          creationDate = r.creationDate,
                                          ratingCount = r.ratingCount,
                                          totalRatingCount = r.totalRatingCount
                                      }).ToList<Rating>();

                    if (user != null)
                    {
                        try
                        {
                            user.userRating = (List<Rating>)userRating;
                            user.overAllRating= (decimal)user.userRating.Average(x => x.ratingCount);
                        }
                        catch (Exception ex)
                        {
                            // Do Nothing Here
                        }

                    }

                }
                response.responseObject = user;
            }
            catch (Exception ex)
            {
                response = new Response(ex);
            }
            return response;
        }

        public bool SendOTPEmail(List<string> To, string Subject, List<string> CC, List<string> BCC, string Body)
        {
            bool Flag = false;
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
                mail.From = new MailAddress("ptapplication.otp@gmail.com", "Personal Trainer", System.Text.Encoding.UTF8);
                mail.To.Add(String.Join(", ", To.ToArray()));
                if (BCC != null && BCC.Count > 0)
                {
                    mail.Bcc.Add(String.Join(", ", BCC.ToArray()));
                }
                mail.Body = Body;
                mail.Subject = Subject;
                SmtpServer.Port = 587;
                SmtpServer.UseDefaultCredentials = false;
                SmtpServer.Credentials = new System.Net.NetworkCredential("ptapplication.otp@gmail.com", "prqhuqpdzamyrhtk");

                SmtpServer.EnableSsl = true;
                mail.IsBodyHtml = true;
                SmtpServer.Send(mail);
            }
            catch (Exception ex)
            {

            }
            return Flag;
        }

        public Response Signup(User user)
        {
            Response response;
            try
            {
                response = new Response();
                if (GetUserByEmail(user.email).responseObject == null)
                {
                    user.userID = Guid.NewGuid();
                    user.createdBy = new Guid();
                    user.updatedBy = new Guid();
                    user.userTypeID = new Guid();
                    user.creationDate = Convert.ToDateTime(System.DateTime.Now);
                    user.postedTime = Convert.ToDateTime(System.DateTime.Now).ToShortDateString();
                    user.postCode = string.IsNullOrEmpty(user.postCode) ? "" : user.postCode;
                    user.fullName = (String.IsNullOrEmpty(user.firstName) ? "" : user.firstName) + " " + (String.IsNullOrEmpty(user.surName) ? "" : user.surName);
                    user.isEmailVerified = false;
                    db.Users.Add(user);
                    Response OTPResponse = GenerateOTP(user);

                    if (OTPResponse.isSuccess)
                    {
                        User newSignup = (User)OTPResponse.responseObject;
                        db.OTPs.Add(new OTP()
                        {
                            oTPID = Guid.NewGuid(),
                            userID = user.userID,
                            type = "Signup OTP",
                            oTPCode = newSignup.oTP,
                            creationDate = Convert.ToDateTime(System.DateTime.Now),
                            expiryDate = Convert.ToDateTime(System.DateTime.Now).AddMinutes(configuration.GetValue<int>("OTPExpiryMinutes")),
                            createdBy = user.userID,
                            isActive = true
                        });
                        db.SaveChanges();
                        response.summary = "User signup successfully.";
                        response.message = "User signup successfully.";
                    }
                    else
                    {
                        response.summary = OTPResponse.summary;
                        response.message = OTPResponse.message;
                    }

                }
                else
                {
                    response.isSuccess = false;
                    response.summary = "User with email " + user.email + " already exists.";
                    response.message = "User with email " + user.email + " already exists.";
                    response.status = (int)ResponseStatus.Forbidden;
                }

            }
            catch (Exception ex)
            {
                response = new Response(ex);
            }
            return response;
        }

        public Response VerifyEmail(User user)
        {
            Response response;
            try
            {
                response = new Response();
                if (GetUserByEmail(user.email).responseObject != null)
                {

                    user.isEmailVerified = true;
                    User Temp = db.Users.Where(x => x.email == user.email).FirstOrDefault();
                    OTP oTP = db.OTPs.Where(x => x.userID == Temp.userID && x.oTPCode == user.oTP).FirstOrDefault();

                    // && x.ExpiryDate <= System.DateTime.Now && x.ExpiryDate >= System.DateTime.Now
                    // Temp.isEmailVerified = true;


                    if (oTP != null)

                    {
                        Temp.isEmailVerified = true;
                        db.Entry(Temp).CurrentValues.SetValues(Temp);
                        db.SaveChanges();
                        response.message = "User Email Verified successfully.";
                        response.summary = "User Email Verified successfully.";
                    }
                    else
                    {
                        response.message = "Invalid OPT.";
                        response.summary = "Invalid OPT.";
                        response.status = (int)ResponseStatus.NotFound;
                        response.isSuccess = false;
                    }


                }
                else
                {
                    response.message = "User with email " + user.email + " not exists.";
                    response.summary = "User with email " + user.email + " not exists.";
                    response.status = (int)ResponseStatus.Forbidden;
                    response.isSuccess = false;
                }
            }
            catch (Exception ex)
            {
                response = new Response(ex);
            }
            return response;
        }

        public Response GenerateOTP(User user)
        {
            Response response;
            try
            {
                response = new Response();
                Random random = new Random();
                int otp = random.Next(1000, 9999);
                user.oTP = otp;
                List<string> to = new List<string>();
                to.Add(user.email);
                string body = @"<h1>Signup Successfully</h1> <p>Below is your OTP. please enter to verify your email.</p> <h3>" + otp + "</h3>";
                SendOTPEmail(to, "Verify OTP", null, null, body);
                response.responseObject = user;
            }
            catch (Exception ex)
            {
                response = new Response(ex);
            }
            return response;
        }



        public Response RegenerateOTP(User user)
        {
            Response response;
            try
            {
                if (!string.IsNullOrEmpty(user.email) && user.userID == null)
                {
                    User Temp = (User)GetUserByEmail(user.email).responseObject;
                    if (Temp != null)
                    {
                        user.userID = Temp.userID;
                    }

                }
                response = new Response();
                Response OTPResponse = GenerateOTP(user);

                if (OTPResponse.isSuccess)
                {
                    User newSignup = (User)OTPResponse.responseObject;
                    db.OTPs.Add(new OTP()
                    {
                        oTPID = Guid.NewGuid(),
                        userID = user.userID,
                        type = "Signup OTP",
                        oTPCode = newSignup.oTP,
                        creationDate = Convert.ToDateTime(System.DateTime.Now),
                        expiryDate = Convert.ToDateTime(System.DateTime.Now).AddMinutes(configuration.GetValue<int>("OTPExpiryMinutes")),
                        createdBy = user.userID,
                        isActive = true
                    });
                    db.SaveChanges();

                    response.message = "OTP regenerated successfully";
                }
            }
            catch (Exception ex)
            {
                response = new Response(ex);
            }
            return response;
        }

        public Response UpdateProfile(User user)
        {
            Response response;
            try
            {
                response = new Response();
                Guid? UID = Utilities.GetIdentity(httpContext).userID;

                User CurrentUser = (User)GetUserByUserID().responseObject;
                if (CurrentUser != null)
                {
                    CurrentUser.profilePicture = user.profilePicture;
                    CurrentUser.firstName = user.firstName;
                    CurrentUser.lastName = user.lastName;
                    CurrentUser.surName = user.surName;
                    CurrentUser.cell = user.cell;
                    CurrentUser.region = user.region;
                    CurrentUser.postCode = user.postCode;
                    CurrentUser.companyName = user.companyName;
                    CurrentUser.companyEmail = user.companyEmail;
                    CurrentUser.companyPhone = user.companyPhone;
                    CurrentUser.website = user.website;
                    CurrentUser.companyLocation = user.companyLocation;
                    CurrentUser.companySize = user.companySize;
                    CurrentUser.yearsInBusiness = user.yearsInBusiness;
                    CurrentUser.companyDescription = user.companyDescription;
                    CurrentUser.serviceLocation1 = user.serviceLocation1;
                    CurrentUser.serviceLocation2 = user.serviceLocation2;
                    CurrentUser.locationShownOnProfile = user.locationShownOnProfile;
                    CurrentUser.latitude = user.latitude;
                    CurrentUser.longitude = user.longitude;
                    CurrentUser.frequencyTime = user.frequencyTime;
                    db.Entry(CurrentUser).CurrentValues.SetValues(CurrentUser);
                    db.SaveChanges();

                    response.summary = "User profile updated successfully.";
                    response.message = "User profile updated successfully.";
                }

                else
                {
                    response.summary = "User Not found.";
                    response.message = "User Not found.";
                    response.status = (int)ResponseStatus.NotFound;
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
