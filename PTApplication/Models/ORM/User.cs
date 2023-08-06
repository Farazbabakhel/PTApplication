using Microsoft.Owin.BuilderProperties;
using System.ComponentModel.DataAnnotations.Schema;

namespace PTApplication.Models.ORM
{
    public class User
    {
        public Guid? userID { get; set; }
        public Guid? userTypeID { get; set; }
        public string? firstName { get; set; }
        public string? lastName { get; set; }
        public string? surName { get; set; }
        public string? fullName { get; set; }
        public string? username { get; set; }
        public string? email { get; set; }
        public string? password { get; set; }
        public string? accountType { get; set; }
        public string? cell { get; set; }
        public string? latitude { get; set; }
        public string? longitude { get; set; }
        public string? region { get; set; }
        public decimal? radius { get; set; }
        public decimal? requiredCreditsForContact { get; set; }
        [NotMapped]
        public decimal? requiredCreditsForContactAfterDiscount { get; set; }
        public string? postCode { get; set; }
        public bool? isEmailVerified { get; set; }
        public string? profilePicture { get; set; }
        public  string? companyName { get; set; }
        public string? companyEmail { get; set; }
        public string? companyPhone { get; set; }
        public string? website { get; set; }
        public string? companyLocation { get; set; }
        public string? companySize { get; set; }
        public string? yearsInBusiness { get; set; }
        public string? companyDescription  { get; set; }
        public string? serviceLocation1 { get; set; }
        public string? serviceLocation2 { get; set; }
        public string? frequencyTime { get; set; }
        public string? postedTime { get; set; }
        public string? firebaseToken { get; set; }
        public string? locationShownOnProfile { get; set; }
        [NotMapped]
        public List<UserTag>? userTags { get; set; }
        [NotMapped]
        public List<Rating>? userRating { get; set; }
        [NotMapped]
        public decimal? overAllRating { get; set; }
        [NotMapped]
        public bool? isBioCompleted { get; set; }
        [NotMapped]
        public int? oTP { get; set; }
        public DateTime? creationDate { get; set; }
        public DateTime? updateDate { get; set; }
        public DateTime? profileCompleteDate { get; set; }
        public Guid? createdBy { get; set; }
        public Guid? updatedBy { get; set; }
        public bool? isProfileCompleted { get; set; }
        public bool? isDiscountApplicable { get; set; }
        public bool? isActive { get; set; }

        public User()
        {
            isBioCompleted = false;
            isActive = true;
            isEmailVerified = false;
            profileCompleteDate = null;
            // By default the isDiscountApplicable property will have true value and need to check only more than 7 days
            // If client request to a PT or 5 PT approch to a client the isDiscountApplicable will be false  
            isDiscountApplicable = false; 
            isProfileCompleted = false;
            userTags = new List<UserTag>();
            userRating= new List<Rating>();
        }








    }
}
