using System.ComponentModel.DataAnnotations.Schema;

namespace PTApplication.Models.ORM
{
    public class UserTag
    {
        public Guid? userTagID { get; set; }
        public Guid? tagID { get; set; }
        public Guid? userID { get; set; }
        [NotMapped]
        public string? name { get; set; }
        [NotMapped]
        public string? description { get; set; }
        public DateTime? updateDate { get; set; }
        public Guid? createdBy { get; set; }
        public Guid? updatedBy { get; set; }
        public bool? isActive { get; set; }
        public UserTag()
        {
            isActive = true;
        }
    }
}
