namespace IBankActorInterface;

using System.Threading.Tasks;
using Dapr.Actors;
using DaprBankActor;

public class AccountBalance
{
    public string AccountId { get; set; }

    public decimal Balance { get; set; }
}

public class WithdrawRequest
{
    public decimal Amount { get; set; }
}
public class DepositRequest
{
    public decimal Amount { get; set; }
    public string AccountId { get; set; }
}

public enum TransferType {
    Withdraw,
    Deposit,
};

public interface IBankActor : IActor
{
    Task<AccountBalance> SetupNewAccount(decimal startingDeposit);
    Task<AccountBalance> GetAccountBalance();
    Task<TransactionResponse> Withdraw(WithdrawRequest withdraw);
    Task<TransactionResponse> Deposit(DepositRequest deposit);   
    Task UnRegisterReoccurring(TransferType type);
    Task RegisterReoccurring(TransferType type, decimal amount);
}