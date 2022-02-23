namespace QuickSpace.Models.Entities
{
    public class Sell
    {
        public int Id { get; set; } 
        public string sellerName { get; set; }
        public string Sticker { get; set; }
        public double Amount { get; set; }
        public bool IsActive { get; set; }
    }
}
