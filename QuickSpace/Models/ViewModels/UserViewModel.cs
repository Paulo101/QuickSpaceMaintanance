using QuickSpace.Data;
using QuickSpace.Models.Entities;

namespace QuickSpace.Models.ViewModels
{
    public class UserViewModel
    {
        public ApplicationUser User { get; set; }
        public Wallet Wallet { get; set; }
    }
}
