using System;
namespace UnitedPayment.API.Models
{
    public class Customer
    {
        public string CustomerId { get; set; }

        public string Name { get; set; }

        public string SurName { get; set; }

        public int BirthDateYear { get; set; }

        public long IdentityNo { get; set; }

        public long IdentityNoVerified { get; set; }

        public bool Status { get; set; }
    }
}
