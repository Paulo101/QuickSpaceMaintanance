using QuickSpace.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace QuickSpace.Models.ViewModels
{
    public class WalletViewModel
    {

        public Wallet Wallet { get; set; }
        public string FullName { get; set; }
        [Required(ErrorMessage="Withdrawal Amount Required")]
        public double Amount { get; set; }  
    }
}
