namespace PTApplication.Models.ORM
{ 
    public class Recharge
    {
        public Guid? rechargeID { get; set; }
        public Guid? userID { get; set; }
        public Guid? clientID { get; set; }
        public Guid? balanceID { get; set; }
        public string? rechargeType { get; set; }
        public string? rechargeReason { get; set; }
        public string? rechargeStatus { get; set; }
        public string? packageCode { get; set; }
        public decimal? amount { get; set; }
        public decimal? credits { get; set; }
        public decimal? openingBalance { get; set; }
        public decimal? closingBalance { get; set; }
        public decimal? openingCredits { get; set; }
        public decimal? closingCredits { get; set; }
        public string? rechargeDate { get; set; }
        public string? transactionRequestJson { get; set; }
        public string? transactionResponseJson { get; set; }
        public string? transactionType { get; set; }
        public DateTime? creationDate { get; set; }
        public DateTime? updateDate { get; set; }
        public Guid? createdBy { get; set; }
        public Guid? updatedBy { get; set; }
        public bool? isActive { get; set; }

        public Recharge()
        {
            isActive = true;
        }
    }

    
}
