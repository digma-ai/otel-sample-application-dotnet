
namespace Sample.MoneyTransfer.API.Domain.Services
{
    public interface ICreditProviderService
    {
        Task<bool> CheckCreditProvider(long accountId);
    }
}