namespace PTApplication.Models.ORM
{
   
    public class PurchaseDetails
    {
        public string purchaseToken { get; set; } // The purchase token received from the Google Play Billing Library
        public string productId { get; set; } // The product ID or SKU of the purchased item
        public string userId { get; set; } // Optional: The user ID or identifier for the purchaser
        public decimal amount { get; set; } // Optional: The purchase amount or price
        public DateTime purchaseDate { get; set; } // Optional: The date and time of the purchase
                                                   // Additional properties as per your requirements
    }
}
