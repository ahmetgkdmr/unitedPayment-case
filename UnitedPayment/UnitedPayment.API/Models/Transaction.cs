using System;
namespace UnitedPayment.API.Models
{
    public class Transaction
    {
        public int TransactionId { get; set; }

        public string CustomerId { get; set; }

        public string OrderId { get; set; }

        public string TypeId { get; set; }

        public string Amount { get; set; }

        public string CardPan { get; set; }

        public string ResponseCode { get; set; }

        public string ResponseMessage { get; set; }

        public bool Status { get; set; }
    }
}