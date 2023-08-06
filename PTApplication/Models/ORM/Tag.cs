namespace PTApplication.Models.ORM
{
    public class Tag
    { 
        public Guid? tagID { get; set; }
        public string? name { get; set; }
        public string? description { get; set; }
        public DateTime? creationDate { get; set; }
        public DateTime? updateDate { get; set; }
        public Guid? createdBy { get; set; }
        public Guid? updatedBy { get; set; }
        public bool? isActive { get; set; }

        public Tag()
        {
            isActive = true;
        }

    }
}
