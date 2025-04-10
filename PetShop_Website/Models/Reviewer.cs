using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PetShop_Website.Models
{
    public class Reviewer
    {
        public int ReviewerID { get; set; }
        public int ProductID { get; set; }
        public int UserID { get; set; }
        public string Comment { get; set; }
        public int Rating { get; set; }
        public DateTime CreatedAt { get; set; }

        // Quan hệ
        public virtual Product Product { get; set; }
        public virtual User User { get; set; }
    }
}