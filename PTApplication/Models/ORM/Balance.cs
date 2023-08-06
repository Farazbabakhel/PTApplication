using System.ComponentModel.DataAnnotations.Schema;

namespace PTApplication.Models.ORM
{ 
    public class Balance
    {
        public Guid? balanceID { get; set; }
        public Guid? userID { get; set; }
        public decimal? amount { get; set; }
        public decimal? credits { get; set; }

        public DateTime? creationDate { get; set; }
        public DateTime? updateDate { get; set; }
        public Guid? createdBy { get; set; }
        public Guid? updatedBy { get; set; }
        public bool? isActive { get; set; }
        [NotMapped]
        public List<Recharge>? rechargeHistory { get; set; }


        public Balance()
        {
            isActive = true;
            rechargeHistory = new List<Recharge>();
        }
    }
}
