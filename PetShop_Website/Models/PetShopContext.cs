using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace PetShop_Website.Models
{
    public class PetShopContext : DbContext
    {
        public PetShopContext() : base("StrC") { }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }


    }
}