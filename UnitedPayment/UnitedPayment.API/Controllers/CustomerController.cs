using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using UnitedPayment.API.Models;
using System.Data.SqlClient;
using UnitedPayment.API.Services;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data;

namespace UnitedPayment.API.Controllers
{
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {

        public IConfiguration _configuration;
        public CustomerController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("CreateCustomer")]
        public string CreateCustomer([FromBody] Customer customer)
        {
            var c = new Customer();
            CustomerService customerService = new CustomerService(new SqlCustomerService());

            if (String.IsNullOrEmpty(customer.CustomerId) || String.IsNullOrEmpty(customer.Name) || String.IsNullOrEmpty(customer.SurName) || customer.BirthDateYear == 0
                || customer.IdentityNo == 0 || customer.IdentityNoVerified == 0)
            {
                return "Lütfen tüm alanları doldurunuz.";
            }

            if (customer.IdentityNo != customer.IdentityNoVerified)
            {
                return "Tc kimlik numarasını farklı girdiniz.";
            }

            /* ***** MAC için visual studio kullandığımdan dolayı WCF Web Service ekleyemedim. Ama eklenildiğinde bu şekilde çalışacaktır.

            tcInfo.KPSPublicSoapClient ti = new tc.Info.KPSPublicSoapClient();
            bool rs = ti.TCKimlikNoDogrula(customer.IdentityNo,customer.Name,customer.SurName,customer.BirthDateYear)
                if (rs)
                 {
                    return "TC Kimlik No Doğru Tabloya Kayıt Yapıldı." *********** (Tablo Yok. Dökümanda bulamadım) ************
                 }
                else
                 {
                    return  "TC Kimlik No Doğru Değil."
                 }
             */
            c.CustomerId = customer.CustomerId;
            c.Name = customer.Name;
            c.SurName = customer.SurName;
            c.BirthDateYear = customer.BirthDateYear;
            c.IdentityNo = customer.IdentityNo;
            c.IdentityNoVerified = customer.IdentityNoVerified;
            c.Status = customer.Status;

            customerService.insertCustomer(customer,_configuration);

            return "Tc kimlik numarası doğru, müşteri kaydı yapıldı.";
        }


        [HttpGet]
        [Route("GetAllCustomers")]
        public string GetAllCustomers()
        {
            CustomerService customerService = new CustomerService(new SqlCustomerService());
            return customerService.GetAllCustomers(_configuration);
        }
    }
}
