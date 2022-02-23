using QuickSpace.Models.Entities;

namespace QuickSpace.Data
{
    public class EFSellRepository
    : RepositoryBase<Sell>, ISellRepository
    {
        public EFSellRepository(ApplicationDbContext appDbContext)
            : base(appDbContext) { }
    }
}
