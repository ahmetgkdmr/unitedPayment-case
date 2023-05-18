using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using UnitedPayment.API.Abstract;
using UnitedPayment.API.Models;

namespace UnitedPayment.API.Services
{
    public class SqlCustomerService : ICustomerService
    {
        public string GetAllCustomers(IConfiguration _configuration)
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

        public void insertCustomer(Customer customer, IConfiguration _configuration)
        {
            SqlConnection con = new SqlConnection(_configuration.GetConnectionString("UnitedPaymentCon").ToString());
            con.Open();
            string sqlCommand = "INSERT INTO customer (customerId, name, surName, birthDateYear, identityNo, status)" + "VALUES (@customerId, @name, @surName, @birthDateYear, @identityNo, @status)";
            SqlCommand cmd = new SqlCommand(sqlCommand, con);

            cmd.Parameters.AddWithValue("customerId", customer.CustomerId);
            cmd.Parameters.AddWithValue("name", customer.Name);
            cmd.Parameters.AddWithValue("surName", customer.SurName);
            cmd.Parameters.AddWithValue("birthDateYear", customer.BirthDateYear);
            cmd.Parameters.AddWithValue("identityNo", customer.IdentityNo);
            cmd.Parameters.AddWithValue("status", customer.Status);
            cmd.ExecuteNonQuery();
        }
    }
}
