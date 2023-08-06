using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PTApplication.Models.ORM
{ 
    public class BioQuestion
    {
        [Key]
        public Guid? questionID { get; set; }
        public int? serialNo { get; set; }
        public string? description { get; set; }
        public bool? isRequired { get; set; }
        [NotMapped]
        public List<BioOptions> bioOptions { get; set; }
        public DateTime? creationDate { get; set; }
        public DateTime? updateDate { get; set; }
        public Guid? createdBy { get; set; }
        public Guid? updatedBy { get; set; }
        public bool? isActive { get; set; }
        public BioQuestion()
        {
            bioOptions=new List<BioOptions>();
            isActive = true;
        }

    }
}
