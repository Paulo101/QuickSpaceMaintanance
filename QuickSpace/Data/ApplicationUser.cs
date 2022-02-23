using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace QuickSpace.Data
{
    public class ApplicationUser: IdentityUser
    {
  
        public string FullName { get; set; }
        public string Country { get; set; }
        
        public string RefferalId { get; set; }
        public string ParentId { get; set; }
        public DateTime CreatedDate { get; set; }
        public string VerificationCode { get; set; }    
    }
}
