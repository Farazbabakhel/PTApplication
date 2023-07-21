namespace PTApplication.Models.ORM
{
    public class PTSetting
    {
        public Guid? pTSettingID { get; set; }
        public Guid? userID { get; set; }

        public string? companyName { get; set; }
        public string? profilePicture { get; set; }
        public string? name { get; set; }
        public string? companyEmail { get; set; }
        public string? companyPhone { get; set; }
        public string? website { get; set; }
        public string? companyBusinessLatitude { get; set; }
        public string? companyBusinessLongitude { get; set; }
        public string? companyBusinessLocation { get; set; }
        public string? companyProfileLatitude { get; set; }
        public string? companyProfileLongitude { get; set; }
        public string? companyProfileLocation { get; set; }
        public string? reasonIfNoLocation { get; set; }
        public string? companySize { get; set; }
        public string? yearsInBusiness { get; set; }
        public string? companyDescription { get; set; }

        public DateTime? creationDate { get; set; }
        public DateTime? updateDate { get; set; }
        public Guid? createdBy { get; set; }
        public Guid? updatedBy { get; set; }
        public bool? isActive { get; set; }

        public PTSetting()
        {
            isActive = true;
        }
    }
}
