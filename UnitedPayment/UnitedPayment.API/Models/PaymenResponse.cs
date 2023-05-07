using System;
namespace UnitedPayment.API.Models
{
    public class PaymenResponse
    {

        public class Result
        {
            public string responseCode { get; set; }
            public string responseMessage { get; set; }
            public string orderId { get; set; }
            public object txnType { get; set; }
            public string txnStatus { get; set; }
            public int vposId { get; set; }
            public string vposName { get; set; }
            public string authCode { get; set; }
            public string hostReference { get; set; }
            public string totalAmount { get; set; }
        }

        public bool fail { get; set; }
        public int statusCode { get; set; }
        public Result result { get; set; }
    }
}
