using System;
using Microsoft.Extensions.Configuration;
using UnitedPayment.API.Abstract;
using UnitedPayment.API.Models;

namespace UnitedPayment.API.Services
{
    public class TransactionService : ITransactionService
    {
        ITransactionService _transactionService;
        public TransactionService(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        public string GetAllLogHistory(IConfiguration _configuration)
        {
            return _transactionService.GetAllLogHistory(_configuration);
            
        }

        public void insertLog(Transaction transaction, IConfiguration _configuration)
        {
            _transactionService.insertLog(transaction, _configuration);
        }
    }
}
