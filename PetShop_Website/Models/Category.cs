using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PetShop_Website.Models
{
    public class Category
    {
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
        public string Descriptiom { get; set; }
        public DateTime CreatedAt { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}