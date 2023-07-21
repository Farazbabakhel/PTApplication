using PTApplication.Models.ORM;
using System.Security.Claims;

namespace PTApplication.Models.Global
{
    public static class Utilities
    {
        public static User GetIdentity(this HttpContext httpContext)
        {
            User user = new User();
            var Identity = httpContext.User as ClaimsPrincipal;
            try
            {
                if (Identity != null)
                {
                    List<Claim> claims = Identity.Claims.ToList();
                    user.userID = new Guid(claims[1].Value);
                    user.fullName = claims[2].Value;
                    user.email = claims[3].Value;

                }
                return user;
            }
            catch (Exception)
            {
                return user;
            }
        }
    }
}
