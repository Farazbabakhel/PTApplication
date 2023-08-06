using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PTApplication.Models.ORM
{ 
    public class Package
    {
        [Key]
        public Guid? packageID { get; set; }

        public string? name { get; set; }
        public string? code { get; set; }
        public decimal? price { get; set; }
        public decimal? priceAfterDiscount { get; set; }
        public decimal? credits { get; set; }
        public int? totalResponses { get; set; }
        public decimal offPercent { get; set; }
        public DateTime? effectiveDateFrom { get; set; }
        public DateTime? effectiveDateTo { get; set; }
        public string? description { get; set; }

        public DateTime? creationDate { get; set; }
        public DateTime? updateDate { get; set; }
        public Guid? createdBy { get; set; }
        public Guid? updatedBy { get; set; }
        public bool? isActive { get; set; }
        public Package()
        {
            isActive = true;
        }
    }
}
