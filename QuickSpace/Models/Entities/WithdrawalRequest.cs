namespace QuickSpace.Models.Entities
{
    public class WithdrawalRequest
    {
        public int Id { get; set; } 
        public string UserEmail { get; set; }    
        public double RequestAmount { get; set; }
        public double AvailableBalance { get; set; }
    }
}
