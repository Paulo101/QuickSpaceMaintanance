using QuickSpace.Data;
using System.ComponentModel.DataAnnotations;

namespace QuickSpace.Models.ViewModels
{
    public class InviteViewModel
    {
        [Required(ErrorMessage="User email address required")]
        public string Email { get; set; }   
        public string RefCode { get; set; }
        public IEnumerable<ApplicationUser> MyMembers { get; set; }
    }
}
