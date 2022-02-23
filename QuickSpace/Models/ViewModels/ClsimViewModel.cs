using System.ComponentModel.DataAnnotations;

namespace QuickSpace.Models.ViewModels
{
    public class ClsimViewModel
    {
        public string Email { get; set; }
        public string FullName { get; set; }
        [Required(ErrorMessage = "Sticker Required")]
        public string Sticker { get; set; }
    }
}
