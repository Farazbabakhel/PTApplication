namespace PTApplication.Models.ORM
{
    public class VerificationResult
    { 
        public bool IsValid { get; set; } // Indicates whether the purchase is valid or not
        public string Message { get; set; } // Optional: A message providing additional details or error information
                                            // Additional properties as per your requirements
    }
}
