using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PetShop_Website.Models
{
    public class CheckoutViewModel
    {
        public List<CartItem> CartItems { get; set; }
        public decimal TotalAmount { get; set; }

        // Các trường để hiển thị và xử lý thông tin thanh toán
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string PaymentMethod { get; set; }
    }
}


