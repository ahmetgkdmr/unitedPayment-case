using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using UnitedPayment.API.Models;
using System.Data.SqlClient;


namespace UnitedPayment.API.Controllers
{
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {

        public readonly IConfiguration _configuration;
        public CustomerController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("CreateCustomer")]
        public string CreateCustomer([FromBody] Customer customer)
        {
            var c = new Customer();

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

            insertCustomer(customer);

            return "Tc kimlik numarası doğru, müşteri kaydı yapıldı.";
        }


        /*

        SATIŞ İŞLEMİNDEN SONRA LOG TABLOSUNA KAYIT ATILIP LOGLAMA YAPILABİLİR. ANCAK DÖKÜMANDA TABLOLARI GÖREMEDİM.
         
         */

        [HttpGet("LoginAndPayment")]
        public async Task<string> LoginAndPayment()
        {
            Transaction transaction = new Transaction();
            string loginToken;
            var request = new HttpRequestMessage(HttpMethod.Post, "https://ppgsecurity-test.birlesikodeme.com:55002/api/ppg/Securities/authenticationMerchant");
            request.Content = new StringContent(JsonSerializer.Serialize(new Login() { Password = "kU8@iP3@", Lang = "TR", Email = "murat.karayilan@dotto.com.tr" }));
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
            requestPayment.Content = new StringContent(JsonSerializer.Serialize(new Payment() { memberId = 1, merchantId = 1894, cardNumber = "4355084355084358", cvv = "000", expiryDateMonth = "12", expiryDateYear = "26", customerId = "1234", userCode = "test", txtnType = "Auth", installmentCount = "1", currency = "949", orderId = "1", amount = "1", rnd = "abcd", hash = hashValue, productId = "000032", totalAmount = "1", productName = "Bilgisayar", commissionRate = "10.00" }));
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
                insertLog(transaction);
                return responseMessage;
            }
            else
            {
                transaction.ResponseCode = "Ödemede Sorun";
                transaction.ResponseMessage = "Ödemede Sorun";
                insertLog(transaction);
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


        void insertCustomer(Customer customer)
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

        void insertLog(Transaction transaction)
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
