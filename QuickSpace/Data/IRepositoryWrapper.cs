namespace QuickSpace.Data
{
    public interface IRepositoryWrapper
    {
        IWalletRepository WalletRepository { get; }
        IWithdrawalRepository WithdrawalRepository { get; }
        ISellRepository SellRepository { get; }
        IEnumerable<ApplicationUser> ApplicationUsers { get; }
    }
}
