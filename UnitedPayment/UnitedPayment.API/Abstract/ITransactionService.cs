using Microsoft.Extensions.Configuration;
using UnitedPayment.API.Models;

namespace UnitedPayment.API.Abstract
{
    public interface ITransactionService
    {
         void insertLog(Transaction transaction, IConfiguration _configuration);
         string GetAllLogHistory(IConfiguration _configuration);
    }
}
