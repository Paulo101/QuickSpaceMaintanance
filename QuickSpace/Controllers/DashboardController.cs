using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using QuickSpace.Data;
using QuickSpace.Models.Entities;
using QuickSpace.Models.ViewModels;

namespace QuickSpace.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly IRepositoryWrapper repository;
        private UserManager<ApplicationUser> userManager;
        public DashboardController(IRepositoryWrapper _repository, UserManager<ApplicationUser> _userManager)
        {
            repository = _repository;
            userManager = _userManager;
        }
        public IActionResult Luno() { return Redirect("https://www.luno.com/help/en-US/articles/11000022930"); }
        public IActionResult Invoice() { return View(); }
        public IActionResult Analytics() { return View(); }
        public IActionResult Profile() { return View(repository.ApplicationUsers.FirstOrDefault(s => s.Email == User.Identity.Name)); }
        public IActionResult Settings() { return View(repository.ApplicationUsers.FirstOrDefault(s => s.Email == User.Identity.Name)); }
        [HttpPost]
        public async Task<IActionResult> Settings(ApplicationUser applicationUser) {
            if (!ModelState.IsValid) {
                ViewBag.Error = "Failed to update profile..";
                return View(applicationUser);
            }
            var appUSer = repository.ApplicationUsers.FirstOrDefault(s => s.Email == User.Identity.Name);
            if (appUSer == null) {
                ViewBag.Error = "Something went wrong..";
                return View(applicationUser);
            }
            appUSer.FullName = applicationUser.FullName;
            appUSer.PhoneNumber = applicationUser.PhoneNumber;
            appUSer.Country = applicationUser.Country;

            await userManager.UpdateAsync(appUSer);
            ViewBag.Success = "Profile Updated Successfully!";
            return View(appUSer);
            
        }
        public IActionResult Sell() { return View(new SellViewModel { 
        Email = User.Identity.Name,
        FullName = repository.ApplicationUsers.FirstOrDefault(s => s.Email == User.Identity.Name).FullName}); }
        [HttpPost]
        public IActionResult Sell(SellViewModel sellViewModel) {
            
            var wallet = repository.WalletRepository.FindAll().FirstOrDefault(s => s.WalletHolder == User.Identity.Name);
            if (sellViewModel.Amount <= 64)
            {
                ViewBag.Error = "Selling Amount cannot be less than $ 65.00";
                return View(new SellViewModel
                {
                    Email = User.Identity.Name,
                    FullName = repository.ApplicationUsers.FirstOrDefault(s => s.Email == User.Identity.Name).FullName
                });
            }
            if (wallet.Interest < sellViewModel.Amount)
            {
                ViewBag.Error = "Sell Amount cannot be greater than Available Interest";
                return View(new SellViewModel
                {
                    Email = User.Identity.Name,
                    FullName = repository.ApplicationUsers.FirstOrDefault(s => s.Email == User.Identity.Name).FullName
                });
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
            var cur = repository.ApplicationUsers.FirstOrDefault(s => s.Email == User.Identity.Name);
            //string body = string.Empty;

            //using (StreamReader reader = new StreamReader("EmailHandler/SellEmail.html"))
            //{
            //    body = reader.ReadToEnd();
            //}
            //body = body.Replace("{UserName}", cur.FullName);
            //body = body.Replace("{Sticker}", sell.Sticker);
            //body = body.Replace("{Amount}", sell.Amount.ToString());
            //EmailHandler.Email.Send(cur.Email, "Withdrawal", body, true);
            
            string msg = $"Hi {cur.FullName}, We have recieved your request to sell ${sell.Amount}. Sticker: {sell.Sticker}. Thank you for your transaction";
            WhatsappAPI.SendMessage.Send(cur.PhoneNumber, msg);
            EmailHandler.Email.Send(cur.Email, "Withdrawal", msg, true);
            ViewBag.Success = "Thank you.. Your Request was submitted successfully. Please keep an eye on your Messages Inbox.";

            return View(new SellViewModel
            {
                Email = User.Identity.Name,
                FullName = cur.FullName
            });
        }
        public IActionResult Index()
        {
            var user = repository.ApplicationUsers.FirstOrDefault(s => s.Email == User.Identity?.Name);
            double weeks = (DateTime.Now - user.CreatedDate).TotalDays / 7;
            ViewBag.Weeks = weeks.ToString("0.0");  
            return View(repository.WalletRepository.FindAll().FirstOrDefault(s => s.WalletHolder == User.Identity.Name));
        }
        public IActionResult Claim() { 
            return View(new ClsimViewModel { Email = User.Identity.Name, FullName = repository.ApplicationUsers.FirstOrDefault(s => s.Email == User.Identity.Name).FullName});
        }
        public IActionResult Auction()
        {
            return View(repository.SellRepository.FindAll());
        }
        [HttpPost]
        public async Task<IActionResult> ClaimAsync(ClsimViewModel clsimView) { 
            var sell = repository.SellRepository.FindAll().FirstOrDefault(s => s.Sticker == clsimView.Sticker);
            if (sell == null) {
                ViewBag.Error = "Invalid sticker provider, please verify. If error persists, contact seller";
                return View(new ClsimViewModel { Email = User.Identity.Name, FullName = repository.ApplicationUsers.FirstOrDefault(s => s.Email == User.Identity.Name).FullName });
            }
            if (!sell.IsActive) {
                ViewBag.Error = "Error occured! The sticker code provided has already been used!";
                return View(new ClsimViewModel { Email = User.Identity.Name, FullName = repository.ApplicationUsers.FirstOrDefault(s => s.Email == User.Identity.Name).FullName });
            }
             var sellerWallet = repository.WalletRepository.FindAll().FirstOrDefault(s => s.WalletHolder == sell.sellerName);
             var buyerWallet = repository.WalletRepository.FindAll().FirstOrDefault(s => s.WalletHolder == User.Identity.Name);

            buyerWallet.Balance += sell.Amount;

            sell.IsActive = false;

            repository.WalletRepository.Update(buyerWallet);
     
            if (sell.sellerName != "no-reply-quickspace@outlook.com")
            {
                sellerWallet.Interest -= sell.Amount;
                repository.WalletRepository.Update(sellerWallet);
            }
            var user = repository.ApplicationUsers.FirstOrDefault(s => s.Email == User.Identity.Name);  
            if(user.ParentId != "0000")
            {
                var reff = repository.ApplicationUsers.FirstOrDefault(u => u.RefferalId == user.ParentId);
                if (reff != null) { 
                    var walletref = repository.WalletRepository.FindAll().FirstOrDefault(s => s.WalletHolder == reff.Email);
                    var COM = sell.Amount * 0.15;
                    walletref.Balance += COM;
                    reff.ParentId = "0000";
                   await userManager.UpdateAsync(reff);
                    repository.WalletRepository.Update(walletref);
                    repository.WalletRepository.Save();
                    
                }
            }
            repository.WithdrawalRepository.Save();
            repository.SellRepository.Save();
            var buyer = repository.ApplicationUsers.FirstOrDefault(s => s.Email == User.Identity.Name);
            var seller = repository.ApplicationUsers.FirstOrDefault(s => s.Email == sell.sellerName);   

            string msg = $"Your Sticker {sell.Sticker} was claimed by {buyer.FullName} ({buyer.Email}), ({buyer.PhoneNumber}) today at {DateTime.UtcNow}. Thank you for your transaction.";

            //string body = string.Empty;

            //using (StreamReader reader = new StreamReader("EmailHandler/ClaimEmail.html"))
            //{
            //    body = reader.ReadToEnd();
            //}
            //body = body.Replace("{UserName}", seller.FullName);
            //body = body.Replace("{Msg}", msg);
            EmailHandler.Email.Send(seller.Email, "Claim", msg, true);

            WhatsappAPI.SendMessage.Send(seller.PhoneNumber, msg);

            msg = $"Congrats! An amount of $ {sell.Amount} has been successfully transfared into your Account!. Thank you for your transaction.";


            //using (StreamReader reader = new StreamReader("EmailHandler/ClaimEmail.html"))
            //{
            //    body = reader.ReadToEnd();
            //}
            //body = body.Replace("{UserName}", buyer.FullName);
            //body = body.Replace("{Msg}", msg);
            EmailHandler.Email.Send(buyer.Email, "Claim", msg, true);
            WhatsappAPI.SendMessage.Send(buyer.PhoneNumber, msg);
            ViewBag.Success = "Thank you.. Your Request was submitted successfully. Please keep an eye on your Messages Inbox.";
            return View(new ClsimViewModel { Email = User.Identity.Name, FullName = repository.ApplicationUsers.FirstOrDefault(s => s.Email == User.Identity.Name).FullName });

        }
        public IActionResult Wallet()
        {
            var user = repository.ApplicationUsers.FirstOrDefault(s => s.Email == User.Identity.Name);
            var walletModel = new WalletViewModel
            {
                FullName = user.FullName,
                Wallet = repository.WalletRepository.FindAll().FirstOrDefault(s => s.WalletHolder == user.Email)
            };
           return View(walletModel);    
        }
        [HttpPost]
        public IActionResult Wallet(WalletViewModel walletModel) 
        {
            var wallet = repository.WalletRepository.FindAll().FirstOrDefault(s => s.WalletHolder == User.Identity.Name);
            walletModel.Wallet = wallet;
            walletModel.FullName = repository.ApplicationUsers.FirstOrDefault(s => s.Email == User.Identity.Name).FullName;
            if (walletModel.Amount == 0)
            {
                ViewBag.Error = "Withdrawl Amount cannot be 0";
                return View(walletModel);
            }
            if (wallet.Interest < walletModel.Amount) {
                ViewBag.Error = "Withdrawl Amount cannot be greater than Available Balance";
                return View(walletModel);
            }

            var withdrawal = new WithdrawalRequest
            {
                AvailableBalance = wallet.Interest,
                RequestAmount = walletModel.Amount,

                UserEmail = User.Identity.Name,
            };
            repository.WithdrawalRepository.Create(withdrawal);
            repository.WithdrawalRepository.Save();

            var users = repository.ApplicationUsers.ToList();
            string body = string.Empty;
            string msg = $"Hi {users.FirstOrDefault(s => s.Email == User.Identity.Name).FullName}. We have recieved your request to withdraw an Amount of ${walletModel.Amount}. Please note that the process may take 2 - 5 working days.";
            //using (StreamReader reader = new StreamReader("EmailHandler/WithdrawalEmail.html"))
            //{
            //    body = reader.ReadToEnd();
            //}
            //body = body.Replace("{UserName}", users.FirstOrDefault(s => s.Email == User.Identity.Name).FullName);
            //body = body.Replace("{Amount}", walletModel.Amount.ToString());
            EmailHandler.Email.Send(User.Identity.Name, "Withdrawal", msg, true);
            WhatsappAPI.SendMessage.Send(users.FirstOrDefault(s => s.Email == User.Identity.Name).PhoneNumber, msg);
            ViewBag.Success = "Thank you.. Your Request was submitted successfully. Please keep an eye on your Messages Inbox.";
            
            return View(walletModel);
        }
        public IActionResult Invite()
        {
            var users = repository.ApplicationUsers.ToList();
            var InviteModel = new InviteViewModel
            {
                RefCode = users.FirstOrDefault(s => s.Email == User.Identity.Name).RefferalId,
                MyMembers = users.Where(s => s.ParentId == users.FirstOrDefault(s => s.Email == User.Identity.Name).RefferalId)
            };
            return View(InviteModel);
        }
        [HttpPost]
        public IActionResult Invite(InviteViewModel model)
        {
            var users = repository.ApplicationUsers.ToList();
            string body = string.Empty;

            //using (StreamReader reader = new StreamReader("EmailHandler/InviteEmail.html"))
            //{
            //    body = reader.ReadToEnd();
            //}
            string refcode = repository.ApplicationUsers.FirstOrDefault(s => s.Email == User.Identity.Name).RefferalId;
            //body = body.Replace("{UserName}", model.Email);
            //body = body.Replace("{RefCode}", users.FirstOrDefault(s => s.Email == User.Identity.Name).RefferalId);
            //string msg = $"Hello, {model.Email}, you have been invited to join QuickSpace, please go to 'quick-space.com/Account/Register?ParentId={refcode}' to create an account.";
            //bool IsSendEmail = EmailHandler.Email.Send(model.Email, "QuickSpace Investment - Welcome", body, true);

            string msgS = $"Hello, { users.FirstOrDefault(s => s.Email == User.Identity.Name)}, you have been invited to join QuickSpace, please go to 'https://quickspaceinv.azurewebsites.net/Account/Register?ParentId={refcode}' to create an account.";
            bool IsSendEmail = EmailHandler.Email.Send(model.Email, "QuickSpace Investment - Welcome", msgS, true);
            if (IsSendEmail)
                ViewBag.Success = $"Your Invitation has been sent to your Friend. Please ask them to check their Messages. quick-space.com/Account/Register?ParentId={refcode}";
            else
                ViewBag.Error = "Failed to send Email, please try again later. If Error Persists, contact Administrator.";

            model = new InviteViewModel
            {
                RefCode = users.FirstOrDefault(s => s.Email == User.Identity.Name).RefferalId,
                MyMembers = users.Where(s => s.ParentId == users.FirstOrDefault(s => s.Email == User.Identity.Name).RefferalId)
            };

            return View(model);
        }
        public IActionResult MySells()
        {
            return View(repository.SellRepository.FindByCondition(s => s.sellerName == User.Identity.Name));
        }
    }
}
