using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PetShop_Website.Models
{
    public class User
    {
        public int UserID { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string PhoneNumber { get; set; }
        public string Role { get; set; }
        public DateTime CreatedAt { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<Reviewer> Reviewers { get; set; }
        public bool IsBlocked { get; set; }

    }
}