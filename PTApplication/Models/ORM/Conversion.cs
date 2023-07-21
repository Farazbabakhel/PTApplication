namespace PTApplication.Models.ORM
{
    public class Conversion
    {
        public Guid? conversionID { get; set; }
        public Guid? rechargeID { get; set; }
        
        public Guid? ptID { get; set; }
        public Guid? clientID { get; set; }

        public DateTime? creationDate { get; set; }
        public DateTime? updateDate { get; set; }
        public Guid? createdBy { get; set; }
        public Guid? updatedBy { get; set; }
        public bool? isActive { get; set; }

        public Conversion()
        {
            isActive = true;
        }
    }
}
