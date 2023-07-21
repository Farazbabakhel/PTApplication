using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PTApplication.Models.DBContext;
using PTApplication.Models.ORM;
using System.Text;

namespace PTApplication.Controllers
{
    [Route("PTAPI/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class GooglePaymentController : ControllerBase
    {
        private readonly PTAppDBContext db;
        public IConfiguration configuration { get; }
        public GooglePaymentController(PTAppDBContext _db, IConfiguration _config)
        {
            db = _db;
            configuration = _config;
        }
        [HttpGet]

        [HttpPost("verify-purchase")]
        public async Task<Response> VerifyPurchase([FromBody] PurchaseDetails purchaseDetails)
        {
            Response response;
            try
            {
                response = new Response();
                // Perform server-side verification
                bool isPurchaseValid = await VerifyPurchaseWithGoogle(purchaseDetails);

                if (isPurchaseValid)
                {
                    // Process the purchase and update your database, grant access, etc.
                    // ...
                    //return Ok("Purchase successfully verified.");

                    response.isSuccess = true;
                    response.message = "Purchase successfully verified.";
                }
                else
                {
                    //return BadRequest("Purchase verification failed.");
                    response.isSuccess = false;
                    response.message = "Purchase verification failed.";
                }


            }
            catch (Exception ex)
            {
                response = new Response(ex);
            }
            return response; ;
        }

        private async Task<bool> VerifyPurchaseWithGoogle(PurchaseDetails purchaseDetails)
        {
            // Implement the logic to verify the purchase with Google Play's billing system
            // You'll need to make HTTP requests to the Google Play Developer API or use Google Play Developer PubSub for verification
            // Refer to Google's documentation for the specific API endpoints and authentication required

            // Example code to send a request to the Google Play Developer API for verification
            var httpClient = new HttpClient();
            var jsonSerializer = new JsonSerializer();

            var requestBody = new
            {
                // Construct the request body with purchase details
                // ...
            };

            var jsonRequest = JsonConvert.SerializeObject(requestBody);
            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync("https://www.googleapis.com/example-verification-endpoint", content);

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var verificationResult = JsonConvert.DeserializeObject<VerificationResult>(jsonResponse);
                return verificationResult.IsValid;
            }
            else
            {
                // Handle the case where verification request failed
                // ...
                return false;
            }
        }
    }
}
