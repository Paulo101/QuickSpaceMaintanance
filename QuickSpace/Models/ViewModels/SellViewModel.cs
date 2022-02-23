using System.ComponentModel.DataAnnotations;

namespace QuickSpace.Models.ViewModels
{
    public class SellViewModel
    {
        public string Email { get; set; }
        public string FullName { get; set; }
        [Required(ErrorMessage = "Sell Amount Required")]
        public double Amount { get; set; }
    }
}
