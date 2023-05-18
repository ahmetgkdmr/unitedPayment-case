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
    public class SqlTransactionService : ITransactionService
    {
        public string GetAllLogHistory(IConfiguration _configuration)
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
        public void insertLog(Transaction transaction, IConfiguration _configuration)
        {

            SqlConnection con = new SqlConnection(_configuration.GetConnectionString("UnitedPaymentCon").ToString());
            con.Open();
            string sqlCommand = "INSERT INTO logHistory (customerId, orderId, amount, cardPan, responseCode, responseMessage, status)" + "VALUES (@customerId, @orderId, @amount, @cardPan, @responseCode, @responseMessage, @status)";
            SqlCommand cmd = new SqlCommand(sqlCommand, con);

            cmd.Parameters.AddWithValue("customerId", transaction.CustomerId);
            cmd.Parameters.AddWithValue("orderId", transaction.OrderId);
            cmd.Parameters.AddWithValue("amount", transaction.Amount);
            cmd.Parameters.AddWithValue("cardPan", transaction.CardPan);
            cmd.Parameters.AddWithValue("responseCode", transaction.ResponseCode);
            cmd.Parameters.AddWithValue("responseMessage", transaction.ResponseMessage);
            cmd.Parameters.AddWithValue("status", transaction.Status);
            cmd.ExecuteNonQuery();
        }
    }
}
