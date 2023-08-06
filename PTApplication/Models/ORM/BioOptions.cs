using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PTApplication.Models.ORM
{ 
    public class BioOptions
    {
        [Key]
        public Guid? optionID { get; set; }
        public Guid? questionID { get; set; }
        public int? serialNo { get; set; }
        public string? description { get; set; }
        public string? type { get; set; }
        public int? points { get; set; }
        public bool? isRequired { get; set; }
        [NotMapped]
        public bool? isChecked { get; set; }
        public DateTime? creationDate { get; set; }
        public DateTime? updateDate { get; set; }
        public Guid? createdBy { get; set; }
        public Guid? updatedBy { get; set; }
        public bool? isActive { get; set; }
        public BioOptions() {
            isActive = true;
            isChecked = false;
        }
    }
}
