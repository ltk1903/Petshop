using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PetShop_Website.Models
{
    public class Product
    {
        public int ProductId { get; set; }
        public int CategoryId { get; set; }

        public string ProductName { get; set; }
        public string Description { get; set; }

        public decimal Price { get; set; }
        public int StockQuantity { get; set; }

        public string ImageURL { get; set; }
        public DateTime CreatedAt { get; set; }

        public virtual Category Category { get; set; }
    }
}