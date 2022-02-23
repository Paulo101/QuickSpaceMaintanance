using QuickSpace.Models.Entities;

namespace QuickSpace.Data
{
    public class EFWithdrawalRepository
  : RepositoryBase<WithdrawalRequest>, IWithdrawalRepository
    {
        public EFWithdrawalRepository(ApplicationDbContext appDbContext)
            : base(appDbContext) { }
    }
}
