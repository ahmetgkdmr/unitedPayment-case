using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using UnitedPayment.API.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text.Json;
using System.Data.SqlClient;
using System.Net.Http;
using UnitedPayment.API.Services;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using Newtonsoft.Json;

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

        [HttpGet("LoginAndPayment")]
        public async Task<string> LoginAndPayment()
        {
            TransactionService transactionService = new TransactionService(new SqlTransactionService());
            Transaction transaction = new Transaction();
            string loginToken;
            var request = new HttpRequestMessage(HttpMethod.Post, "https://ppgsecurity-test.birlesikodeme.com:55002/api/ppg/Securities/authenticationMerchant");
            request.Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(new Login() { Password = "kU8@iP3@", Lang = "TR", Email = "murat.karayilan@dotto.com.tr" }));
            request.Content.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/json");

            var loginClient = new HttpClient();
            loginClient.BaseAddress = new Uri("https://ppgsecurity-test.birlesikodeme.com:55002/api/ppg/Securities/authenticationMerchant");
            loginClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("aplication/json"));
            var response = await loginClient.SendAsync(request);


            if (response.IsSuccessStatusCode)
            {
                var responseStream = await response.Content.ReadAsStringAsync();
                var login = Newtonsoft.Json.JsonConvert.DeserializeObject<LoginResponse>(responseStream);
                loginToken = login.result.token;
            }
            else
            {
                return "Login olurken hata oldu.";
            }

            var hashValue = CreateHash();

            var paymentClient = new HttpClient();
            paymentClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginToken);
            var requestPayment = new HttpRequestMessage(HttpMethod.Post, "https://ppgpayment-test.birlesikodeme.com:20000/api/ppg/Payment/NoneSecurePayment");
            requestPayment.Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(new Payment() { memberId = 1, merchantId = 1894, cardNumber = "4355084355084358", cvv = "000", expiryDateMonth = "12", expiryDateYear = "26", customerId = "1234", userCode = "test", txtnType = "Auth", installmentCount = "1", currency = "949", orderId = "1", amount = "1", rnd = "abcd", hash = hashValue, productId = "000032", totalAmount = "1", productName = "Bilgisayar", commissionRate = "10.00" }));
            requestPayment.Content.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/json");


            paymentClient.BaseAddress = new Uri("https://ppgpayment-test.birlesikodeme.com:20000/api/ppg/Payment/NoneSecurePayment");
            paymentClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("aplication/json"));
            var responsePayment = await paymentClient.SendAsync(requestPayment);

            transaction.CustomerId = "1234";
            transaction.OrderId = "1";
            transaction.Amount = "1";
            transaction.CardPan = "4355084355084358";


            if (responsePayment.IsSuccessStatusCode)
            {
                string responseMessage;
                var responseStream2 = await responsePayment.Content.ReadAsStringAsync();
                var payment = Newtonsoft.Json.JsonConvert.DeserializeObject<PaymenResponse>(responseStream2);
                responseMessage = payment.result.responseMessage;
                transaction.ResponseCode = payment.result.responseCode;
                transaction.ResponseMessage = payment.result.responseMessage;
                transaction.Status = true;
                transactionService.insertLog(transaction, _configuration);
                return responseMessage;
            }
            else
            {
                transaction.ResponseCode = "Ödemede Sorun";
                transaction.ResponseMessage = "Ödemede Sorun";
                transactionService.insertLog(transaction, _configuration);
                return "Ödemede hata oldu.";
            }


            string CreateHash()
            {
                var hashString = $"{"SKI0NDHEUP60J7QVCFATP9TJFT2OQFSO"}{"441"}{"abcd"}{"Auth"}{"1"}{"1234"}{"1"}";

                System.Security.Cryptography.SHA512 s512 = System.Security.Cryptography.SHA512.Create();

                System.Text.UnicodeEncoding ByteConverter = new System.Text.UnicodeEncoding();

                byte[] bytes = s512.ComputeHash(ByteConverter.GetBytes(hashString));

                var hash = System.BitConverter.ToString(bytes).Replace("-", "");

                return hash;
            }


        }

        [HttpGet]
        [Route("GetAllLogHistory")]
        public string GetAllLogHistory()
        {
            TransactionService transactionService = new TransactionService(new SqlTransactionService());
            return transactionService.GetAllLogHistory(_configuration);
        }

    }
}
