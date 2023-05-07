using System;
namespace UnitedPayment.API.Models
{
    public class LoginResponse
    {
        public class Result
        {
            public int userId { get; set; }

            public string token { get; set; }
        }
        public bool fail { get; set; }

        public int statusCode { get; set; }

        public Result result { get; set; }

        public int count { get; set; }

        public string responseCode { get; set; }

        public string responseMessage { get; set; }

    }
}
