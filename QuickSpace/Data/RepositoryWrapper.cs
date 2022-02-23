namespace QuickSpace.Data
{
    public class RepositoryWrapper : IRepositoryWrapper
    {
        private IWalletRepository _walletrepository;
        private IWithdrawalRepository _withdrawalrepository;
        private ISellRepository _sellrepository;
        private ApplicationDbContext _context;
        public RepositoryWrapper(ApplicationDbContext dbContext) {
            _context = dbContext;
        }
        public ISellRepository SellRepository
        {
            get {
                if (_sellrepository == null)
                { 
                    _sellrepository = new EFSellRepository(_context);
                }
                return _sellrepository;
            }
        }
        public IWalletRepository WalletRepository
        {
            get {
                if (_walletrepository == null)
                {
                    _walletrepository = new EFWalletRepository(_context);
                }

               return _walletrepository;
            }
        }

        public IEnumerable<ApplicationUser> ApplicationUsers 
        { 
            get {
                return _context.Users.ToList();
            }
        }

        public IWithdrawalRepository WithdrawalRepository {
            get {
                if (_withdrawalrepository == null)
                {
                    _withdrawalrepository = new EFWithdrawalRepository(_context);
                }

                return _withdrawalrepository;
            }
        }
    }
}
