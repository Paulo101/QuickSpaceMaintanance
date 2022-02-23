using QuickSpace.Models.Entities;

namespace QuickSpace.Data
{
    public class EFWalletRepository
    : RepositoryBase<Wallet>, IWalletRepository
    {
        public EFWalletRepository(ApplicationDbContext appDbContext)
            : base(appDbContext) { }
    
    }
}
