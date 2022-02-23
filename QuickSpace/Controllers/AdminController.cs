using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using QuickSpace.Data;
using QuickSpace.Models.Entities;
using QuickSpace.Models.ViewModels;

namespace QuickSpace.Controllers
{
    [Authorize(Roles ="Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> UserManager;
        private readonly SignInManager<ApplicationUser> SignInManager;
        private readonly IRepositoryWrapper repository;
        public AdminController(UserManager<ApplicationUser> _UserManager,
            SignInManager<ApplicationUser> _SignInManager, IRepositoryWrapper _repository)
        {
            UserManager = _UserManager;
            SignInManager = _SignInManager;
            repository = _repository;
        }
        public IActionResult Index()
        {
            double balances = 0, sales = 0;

            foreach (var wallet in repository.WalletRepository.FindAll())
            {
                balances += wallet.Balance;
            }
            foreach (var sell in repository.SellRepository.FindAll())
                sales += sell.Amount;

            return View(new AdminViewModel { 
                Balances = balances,
                Sales = sales,
                Users = repository.ApplicationUsers.Count()
            });
        }
        public IActionResult UserDetails(string id)
        {
            var user = repository.ApplicationUsers.FirstOrDefault(s => s.Id == id);
            if (user == null) {
                ViewBag.Error = "Something went wrong viewing the user";
                return View();
            }
            var wallet = repository.WalletRepository.FindAll().FirstOrDefault(S => S.WalletHolder == user.Email);
            return View(new UserViewModel { User = user, Wallet = wallet });
        }
        public IActionResult Auction()
        {
            return View(repository.SellRepository.FindAll());
        }
        [HttpGet]
        public IActionResult MySells()
        {
            return View(repository.SellRepository.FindByCondition(s=> s.sellerName == User.Identity.Name));
        }
        public IActionResult Users()
        {
            return View(repository.ApplicationUsers.Where(s => s.Email != User.Identity.Name));
        }
        public IActionResult Withdrawal()
        {
            return View(repository.WithdrawalRepository.FindAll());
        }
        [HttpPost]
        public IActionResult Withdrawal(int id)
        {
            var withdraw = repository.WithdrawalRepository.GetById(id);
            if(withdraw == null)
            {
                ViewBag.Error = "Failed to Approve this Withdrawal, Please try again. If error persists contact support engineer!";
                return View(repository.WithdrawalRepository.FindAll());
            }
            var userwallet = repository.WalletRepository.FindAll().FirstOrDefault(S => S.WalletHolder == withdraw.UserEmail);

            userwallet.Withdrawal += withdraw.RequestAmount;
            userwallet.Interest -= withdraw.RequestAmount;

            repository.WalletRepository.Update(userwallet); 
            repository.WithdrawalRepository.Delete(withdraw);
            repository.WithdrawalRepository.Save();
            repository.WalletRepository.Save();

            string msg = $"Congrats!! Your withdrawal of ${withdraw.RequestAmount} was transfered successfully! Thank you for your transaction.";

            //string body = string.Empty;

            //using (StreamReader reader = new StreamReader("EmailHandler/ClaimEmail.html"))
            //{
            //    body = reader.ReadToEnd();
            //}
            var user = repository.ApplicationUsers.FirstOrDefault(s => s.Email == withdraw.UserEmail);
            //body = body.Replace("{UserName}", user.FullName);
            //body = body.Replace("{Msg}", msg);
            EmailHandler.Email.Send(user.Email, "Withdrawal", msg, true);
            WhatsappAPI.SendMessage.Send(user.PhoneNumber, msg);

            msg = $"This Confirms that you approved a withdrawal request of ${withdraw.RequestAmount}. from {user.FullName} ({user.Email}), ({user.PhoneNumber})";


            //using (StreamReader reader = new StreamReader("EmailHandler/ClaimEmail.html"))
            //{
            //    body = reader.ReadToEnd();
            //}
            //body = body.Replace("{UserName}", "QuickSpace Admin");
            //body = body.Replace("{Msg}", msg);
            EmailHandler.Email.Send(User.Identity.Name, "Withdrawal", msg, true);
            WhatsappAPI.SendMessage.Send("+27712909794" ,msg);
            ViewBag.Success = "Thank you.. Your Request was submitted successfully. Please keep an eye on your Messages Inbox.";

            return View(repository.WithdrawalRepository.FindAll());
        }
        [HttpGet]
        public IActionResult Sell()
        {
            return View(new SellViewModel { FullName = "Quick-Space Admin",
            Email = User.Identity.Name});
        }
        [HttpPost]
        public IActionResult Sell(SellViewModel sellViewModel)
        {
            sellViewModel.FullName = "Quick-Space Admin";
            sellViewModel.Email = User.Identity.Name;
            if (sellViewModel.Amount <= 64)
            {
                ViewBag.Error = "Selling Amount cannot be less than $ 65.00";
                return View(sellViewModel);
            }
            var sell = new Sell
            {
                Amount = sellViewModel.Amount,
                sellerName = User.Identity.Name,
                IsActive = true,
                Sticker = Guid.NewGuid().ToString("N").Substring(0, 5).ToString(),

            };
            repository.SellRepository.Create(sell);
            repository.SellRepository.Save();

            string body = string.Empty;
            var cur = repository.ApplicationUsers.FirstOrDefault(s => s.Email == User.Identity.Name);
            //using (StreamReader reader = new StreamReader("EmailHandler/SellEmail.html"))
            //{
            //    body = reader.ReadToEnd();
            //}
            //body = body.Replace("{UserName}", sellViewModel.FullName);
            //body = body.Replace("{Sticker}", sell.Sticker);
            //body = body.Replace("{Amount}", sell.Amount.ToString());
        

         
            string msg = $"Hi {cur.FullName}, We have recieved your request to sell ${sell.Amount}. Sticker: {sell.Sticker}. Thank you for your transaction";
            WhatsappAPI.SendMessage.Send("+27712909794", msg);
            EmailHandler.Email.Send(cur.Email, "Withdrawal", msg, true);
            ViewBag.Success = "Thank you.. Your Request was submitted successfully. Please keep an eye on your Messages Inbox.";

            return View(sellViewModel);
        }
        [AllowAnonymous]
        [HttpGet]
        public IActionResult LogOut()
        {
            return View();

        }
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> LogOut(string Controller)
        {
            await SignInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");

        }
    }
}
