namespace QuickSpace.Models.Entities
{
    public class Wallet
    {
        public int WalletId { get; set; }
        public string WalletHolder { get; set; }
        public double Balance { get; set; } 
        public double Interest { get; set; }
        public double Withdrawal { get; set; }
        public double Deposit { get; set; }
    }
}
