namespace PTApplication.Models.ORM
{
    public class RequestReply
    {
        public Guid? requestReplyID { get; set; }
        public Guid? pTID { get; set; }
        public Guid? clientID { get; set; }
        public string? status { get; set; }
        public int? statusID { get; set; }
        public string? requestMessage { get; set; }
        public string? ptComments { get; set; }

        public DateTime? creationDate { get; set; }
        public DateTime? updateDate { get; set; }
        public Guid? createdBy { get; set; }
        public Guid? updatedBy { get; set; }
        public bool? isActive { get; set; }
        public RequestReply()
        {
            isActive = true;
        }
    }

    public class RequestReplyReport:User
    {
        public Guid? requestReplyID { get; set; }
        public Guid? pTID { get; set; }
        public Guid? clientID { get; set; }
        public string? status { get; set; }
        public int? statusID { get; set; }
        public string? requestMessage { get; set; }
        public string? ptComments { get; set; }

        public DateTime? creationDate { get; set; }
        public DateTime? updateDate { get; set; }
        public Guid? createdBy { get; set; }
        public Guid? updatedBy { get; set; }
        public bool? isActive { get; set; }
        public RequestReplyReport()
        {
            isActive = true;
        }
    }
}
