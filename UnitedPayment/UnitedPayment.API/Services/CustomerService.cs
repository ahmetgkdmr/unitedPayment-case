using Microsoft.Extensions.Configuration;
using UnitedPayment.API.Abstract;
using UnitedPayment.API.Models;

namespace UnitedPayment.API.Services
{
    public class CustomerService : ICustomerService
    {
        ICustomerService _customerService;
        public CustomerService(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        public string GetAllCustomers(IConfiguration _configuration)
        {
            return _customerService.GetAllCustomers(_configuration);
        }

        public void insertCustomer(Customer customer, IConfiguration _configuration)
        {
            _customerService.insertCustomer(customer, _configuration);
        }
    }
}
