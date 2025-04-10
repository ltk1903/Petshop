using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PetShop_Website.Models
{
    public class Payment
    {
        public int PaymentID { get; set; }
        public int OrderID { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentStatus { get; set; }
        public string TransactionID { get; set; }
        public DateTime PaymentDate { get; set; }

        public virtual Order Order { get; set; }
    }
}