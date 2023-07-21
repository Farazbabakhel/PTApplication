using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Asn1.Ocsp;
using PTApplication.Models.DBContext;
using PTApplication.Models.ORM;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PTApplication.Models.ViewModels.Users
{
    public class LoginViewModel
    {
        private readonly PTAppDBContext db;
        private readonly IConfiguration configuration;
        private readonly HttpContext httpContext;
        public LoginViewModel(PTAppDBContext _db, IConfiguration _configuration, HttpContext _httpContext)
        {
            db = _db;
            configuration = _configuration;
            httpContext = _httpContext;
        }

        public Response Login(User user)
        {
            Response response;
            try
            {
                string AdminEmail = configuration["AdminEmail"];
                string AdminPassword = configuration["AdminPassword"];
                response = new Response();
                if (user.email != AdminEmail || (user.email == AdminEmail && user.password == AdminPassword))
                {
                    User result = db.Users.Where(x => x.email == user.email && x.password.Equals(user.password) && x.isActive == true).FirstOrDefault();
                    if (result != null)
                    {
                        if(result.isEmailVerified==true)
                        {
                            result.isBioCompleted = db.UserBioAnswers.Any(x=>x.userID==result.userID && x.isActive==true);
                            var permClaims = new List<Claim>();
                            permClaims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
                            permClaims.Add(new Claim("userID", Convert.ToString(result.userID)));
                            permClaims.Add(new Claim("fullName", result.fullName));
                            permClaims.Add(new Claim("email", result.email));
                            permClaims.Add(new Claim("firebaseToken", String.IsNullOrEmpty(user.firebaseToken)?"": user.firebaseToken));
                            //permClaims.Add(new Claim("postCode", result.postCode));
                            string Key = configuration["jWTKey"];
                            var issuer = configuration["jWTIssuer"];
                            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Key));
                            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
                            var token = new JwtSecurityToken(issuer,
                                issuer,
                                permClaims,
                                expires: DateTime.Now.AddDays(1),
                                signingCredentials: credentials
                                );
                            var jwt_token = new JwtSecurityTokenHandler().WriteToken(token);
                            response.responseObject = new { token = jwt_token, userInfo = result };

                            User previousUserToken= db.Users.Where(x => x.firebaseToken == user.firebaseToken).FirstOrDefault();
                            if (previousUserToken != null)
                            {
                                previousUserToken.firebaseToken = "";
                                db.Entry(previousUserToken).CurrentValues.SetValues(previousUserToken);
                                db.SaveChanges();
                            }
                            User currentUserToken = db.Users.Where(x => x.userID == result.userID).FirstOrDefault();
                            if(currentUserToken!=null)
                            {
                                currentUserToken.firebaseToken = user.firebaseToken;
                                db.Entry(currentUserToken).CurrentValues.SetValues(currentUserToken);
                                db.SaveChanges();
                                result.firebaseToken = user.firebaseToken;
                            }
                        }
                        else
                        {
                            response.status = (int)PTApplication.Models.Global.ResponseStatus.Forbidden;
                            response.isSuccess = false;
                            response.heading = "Email Not Verified";
                            response.message = "Your email is not verified.";
                        }
                        

                    }
                    else
                    {
                        response.status = (int)PTApplication.Models.Global.ResponseStatus.NotFound;
                        response.isSuccess = false;
                        response.heading = "Invalid Username/Password";
                        response.message = "Username And Password Do Not Match.";
                    }
                }
                else
                {
                    response.status = (int)PTApplication.Models.Global.ResponseStatus.Forbidden;
                    response.isSuccess = false;
                    response.heading = "Email  Not Verified";
                    response.message = "Your Email Is Not Verified Yet.";
                }
                return response;
            }
            catch (Exception ex)
            {
                return new Response(ex);
            }
        }
    }
}
