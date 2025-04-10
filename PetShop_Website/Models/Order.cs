using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PetShop_Website.Models
{
    public class Order
    {
        public int OrderID { get; set; }

        // Khóa ngoại đến User
        public int UserID { get; set; }
        public virtual User User { get; set; }

        public decimal TotalAmount { get; set; }
        public string OrderStatus { get; set; }
        public DateTime OrderDate { get; set; }

        // Quan hệ với chi tiết đơn hàng
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        public virtual Payment Payment { get; set; }
    }
}