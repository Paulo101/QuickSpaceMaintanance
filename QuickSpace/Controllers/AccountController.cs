using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using QuickSpace.Data;
using QuickSpace.Models.Entities;
using QuickSpace.Models.ViewModels;

namespace QuickSpace.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> UserManager;
        private readonly SignInManager<ApplicationUser> SignInManager;
        private readonly IRepositoryWrapper repository;
        public AccountController(UserManager<ApplicationUser> _UserManager,
            SignInManager<ApplicationUser> _SignInManager, IRepositoryWrapper _repository)
        {
            UserManager = _UserManager;
            SignInManager = _SignInManager;
            repository = _repository;
        }
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Verification()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerificationAsync(VerificationViewModel verification)
        {
            var user = repository.ApplicationUsers.FirstOrDefault(s => s.Email == User.Identity.Name);

            if (user != null)
            {
                if (verification.VerificationCode != user.VerificationCode)
                {
                    ViewBag.Error = "Invalid Verification Code Provided. Please try again!";
                    return View(verification);
                }
                user.EmailConfirmed = true;
                await UserManager.UpdateAsync(user);
                ViewBag.Success = "Verification Successful";
                return RedirectToAction("Index", "Dashboard");

            }
            ViewBag.Error = "Verification Failed! Please try again!";
            return View(verification);
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            var user = await UserManager.FindByEmailAsync(model.Email);
         

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);

            if (user != null)
            {
                if (!await UserManager.IsEmailConfirmedAsync(user))
                {
                    return RedirectToAction("Verification");

                }

            }

            if (result.Succeeded)
            {
                if (user.Email.Equals("no-reply-quickspace@outlook.com"))
                    return RedirectToAction("Index", "Admin");
                return RedirectToAction("Index", "Dashboard");
            }
            else
            {
                ModelState.AddModelError("", "Invalid login attempt");
                return View(model);
            }
        }
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Register(string? ParentId)
        {
            string reff = ParentId == null ? "0000" : ParentId;

            if (ParentId != null)
                ViewBag.ParentId = "You were Invited to Join Quick Space, please do not change the Referral Code: " + reff;
            return View(new RegisterViewModel { ParentId = reff });
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {

            if (ModelState.IsValid)
            {
                if (model.Consent)
                {
                    string guid = Guid.NewGuid().ToString("N").Substring(0, 8);

                    var user = new ApplicationUser 
                    {
                        UserName = model.Email, 
                        Email = model.Email,
                        FullName = model.FullName,
                        Country = model.Country,
                        PhoneNumber = model.Number,
                        ParentId = model.ParentId,
                        RefferalId = guid.ToString(),
                        CreatedDate = DateTime.Now,
                        VerificationCode = new Random().Next(111111, 999999).ToString(),
                        
                    };
                    
                    var result = await UserManager.CreateAsync(user, model.Password);
                    if (result.Succeeded)
                    {
                        var wallet = new Wallet {
                        WalletHolder = user.Email,
                        Balance = 0,
                        Interest = 0,
                        Deposit = 0,
                        Withdrawal = 0
                    };
                        repository.WalletRepository.Create(wallet);
                        repository.WalletRepository.Save();
                        // await UserManager.AddToRoleAsync(user, "Student");
                        //string code = await UserManager.GenerateEmailConfirmationTokenAsync(user);
                        //var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);


                        //string message = getConfirmationHTLM(callbackUrl, user.UserName);
                        //string nonHtlmessage = $"Hi { model.UserName}\n\nWe are happy to have you. Please Click on the link below to VERIFY your account\n\n{ callbackUrl}\n\nThis mail is sent from an unattended mail box - do not reply.\n\nKind Regards,\nPaulo NGOVE -WeThink Central Application";

                        //string body = string.Empty;
                        //using (StreamReader reader = new StreamReader("EmailHandler/WelcomeEmail.html"))
                        //{
                        //    body = reader.ReadToEnd();
                        //}
                        //body = body.Replace("{UserName}", model.FullName);
                        //body = body.Replace("{Code}", user.VerificationCode);
                        //EmailHandler.Email.Send(model.Email, "QuickSpace Account - Welcome", body, true);

                        string nonHtlmessage = $"Hi { model.FullName}, \n\nQuickSpace - We are happy to have you. Please use {user.VerificationCode} to Verify your account.";
                        WhatsappAPI.SendMessage.Send(model.Number, nonHtlmessage);
                        EmailHandler.Email.Send(model.Email, "QuickSpace Account - Welcome", nonHtlmessage, true);

                        return RedirectToAction("Login", "Account");
                      
                    }
                    AddErrors(result);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var item in result.Errors)
                ModelState.AddModelError(string.Empty, item.Description);
           
        }
        [AllowAnonymous]
        [HttpGet]
        public IActionResult LogOut() {
            return View();

        }
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> LogOut(string Controller)
        {
            await SignInManager.SignOutAsync();
            return RedirectToAction("Login");

        }
    }

}
