namespace PTApplication.Models.ORM
{
    public class OTP
    {
        public Guid? oTPID { get; set; }
        public Guid? userID { get; set; }
        public string? type { get; set; }
        public int? oTPCode { get; set; }
        public DateTime? creationDate { get; set; }
        public DateTime? expiryDate { get; set; }
        public DateTime? updateDate { get; set; }
        public Guid? createdBy { get; set; }
        public Guid? updatedBy { get; set; }
        public bool?  isActive { get; set; }
        public OTP()
        {
            isActive = true;
        }
    }
}
