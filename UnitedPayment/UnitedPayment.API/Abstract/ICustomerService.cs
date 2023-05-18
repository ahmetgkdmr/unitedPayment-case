using Microsoft.Extensions.Configuration;
using UnitedPayment.API.Models;

namespace UnitedPayment.API.Abstract
{
    public interface ICustomerService
    {
        void insertCustomer(Customer customer, IConfiguration _configuration);
        string GetAllCustomers(IConfiguration _configuration);
    }
}
