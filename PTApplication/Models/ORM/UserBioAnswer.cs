using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PTApplication.Models.ORM
{
    public class UserBioAnswer
    {
        [Key]
        public Guid? userBioAnswerID { get; set; }
        public Guid? userID { get; set; }
        public Guid? questionID { get; set; }
        public Guid? optionID { get; set; }
        public int? serialNo { get; set; }
        public string? answerDescription { get; set; }

        [NotMapped]
        public int? questionSerialNo { get; set; }
        [NotMapped]
        public string? questionDescription { get; set; }
        [NotMapped]
        public int? optionSerialNo { get; set; }
        [NotMapped]
        public string? optionDescription { get; set; }
        [NotMapped]
        public BioQuestion? bioQuestion { get; set; }
        [NotMapped]
        public BioOptions? bioOptions { get; set; }

        public DateTime? creationDate { get; set; }
        public DateTime? updateDate { get; set; }
        public Guid? createdBy { get; set; }
        public Guid? updatedBy { get; set; }
        public bool? isActive { get; set; }
        public UserBioAnswer()
        {
            isActive = true;
            bioQuestion = new BioQuestion();
            bioOptions = new BioOptions();
        }

    }
}
