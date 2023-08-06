using PTApplication.Controllers;
using System.ComponentModel.DataAnnotations.Schema;

namespace PTApplication.Models.ORM
{ 
    public class Rating
    {
        public Guid? ratingID { get; set; }
        public Guid? ratingForUserID { get; set; }
        public Guid? ratingByUserID { get; set; }
        public decimal? ratingCount { get; set; }
        public decimal? totalRatingCount { get; set; }
        public string? comments { get; set; }
        public string? description { get; set; }
        [NotMapped]
        public string? userName { get; set; }

        public DateTime? creationDate { get; set; }
        public DateTime? updateDate { get; set; }
        public Guid? createdBy { get; set; }
        public Guid? updatedBy { get; set; }
        public bool? isActive { get; set; }


        public Rating()
        {
            isActive = true;
            totalRatingCount = 5;
        }
    }
}
