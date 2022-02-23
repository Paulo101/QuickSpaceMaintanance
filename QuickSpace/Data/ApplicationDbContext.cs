using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using QuickSpace.Models.Entities;

namespace QuickSpace.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<WithdrawalRequest> WithdrawalRequests { get; set; }
        public DbSet<Sell> Sells { get; set; }
        public static async Task CreateDefaultAdmin(IServiceProvider serviceProvider)
        {
            //initializing custom roles 
            var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var UserManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            string[] roleNames = { "Admin", "Member" };
            IdentityResult roleResult;

            foreach (var roleName in roleNames)
            {
                var roleExist = await RoleManager.RoleExistsAsync(roleName);
                // ensure that the role does not exist
                if (!roleExist)
                {
                    //create the roles and seed them to the database: 
                    roleResult = await RoleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // find the user with the admin email 
            var _user = await UserManager.FindByEmailAsync("no-reply-quickspace@outlook.com");

            // check if the user exists
            if (_user == null)
            {
                //Here you could create the super admin who will maintain the web app
                var poweruser = new ApplicationUser
                {
                    UserName = "no-reply-quickspace@outlook.com",
                    Email = "no-reply-quickspace@outlook.com",
                    CreatedDate = DateTime.Now,
                    Country = "ZA",
                    EmailConfirmed = true,
                    FullName = "Quick-Space Admin",
                    ParentId = "0000",
                    RefferalId = "0000",
                    VerificationCode = "0000"
                };
                string adminPassword = "Secret123$";

                var createPowerUser = await UserManager.CreateAsync(poweruser, adminPassword);
                if (createPowerUser.Succeeded)
                {
                    //here we tie the new user to the role
                    await UserManager.AddToRoleAsync(poweruser, "Admin");

                }
            }
        }
    }
}