using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using UnitedPayment.API.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
namespace UnitedPayment.API.Controllers
{
    [Route("api/[controller]")]
    public class TransactionController : ControllerBase
    {
        public readonly IConfiguration _configuration;
        public TransactionController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        [Route("GetAllCustomers")]
        public string GetAllCustomers()
        {
            SqlConnection con = new SqlConnection(_configuration.GetConnectionString("UnitedPaymentCon").ToString());
            SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM customer", con);
            DataTable dt = new DataTable();
            da.Fill(dt);
            List<Customer> customerList = new List<Customer>();

            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Customer cs = new Customer();
                    cs.CustomerId = Convert.ToString(dt.Rows[i]["customerId"]);
                    cs.Name = Convert.ToString(dt.Rows[i]["name"]);
                    cs.SurName = Convert.ToString(dt.Rows[i]["surName"]);
                    cs.BirthDateYear = Convert.ToInt32(dt.Rows[i]["birthDateYear"]);
                    cs.IdentityNo = Convert.ToInt64(dt.Rows[i]["identityNo"]);
                    cs.Status = Convert.ToBoolean(dt.Rows[i]["status"]);
                    customerList.Add(cs);
                }
            }
            if (customerList.Count > 0)
            {
                return JsonConvert.SerializeObject(customerList);
            }
            else
            {
                return "Müşteri verilerini çekerken hata meydana geldi";
            }
        }

        [HttpGet]
        [Route("GetAllLogHistory")]
        public string GetAllLogHistory()
        {
            SqlConnection con = new SqlConnection(_configuration.GetConnectionString("UnitedPaymentCon").ToString());
            SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM logHistory", con);
            DataTable dt = new DataTable();
            da.Fill(dt);
            List<Transaction> transactionList = new List<Transaction>();

            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Transaction ts = new Transaction();
                    ts.CustomerId = Convert.ToString(dt.Rows[i]["customerId"]);
                    ts.OrderId = Convert.ToString(dt.Rows[i]["orderId"]);
                    ts.Amount = Convert.ToString(dt.Rows[i]["amount"]);
                    ts.CardPan = Convert.ToString(dt.Rows[i]["cardPan"]);
                    ts.ResponseCode = Convert.ToString(dt.Rows[i]["responseCode"]);
                    ts.ResponseMessage = Convert.ToString(dt.Rows[i]["responseMessage"]);
                    ts.Status = Convert.ToBoolean(dt.Rows[i]["status"]);
                    transactionList.Add(ts);
                }
            }
            if (transactionList.Count > 0)
            {
                return JsonConvert.SerializeObject(transactionList);
            }
            else
            {
                return "Ödeme kayıt bilgilerini çekerken hata meydana geldi";
            }
        }

    }
}
