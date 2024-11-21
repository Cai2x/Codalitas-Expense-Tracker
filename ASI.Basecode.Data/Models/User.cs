using System;
using System.Collections.Generic;

namespace ASI.Basecode.Data.Models
{
    public partial class User
    {
        public User()
        {
            Categories = new HashSet<Category>();
            Tokens = new HashSet<Token>();
        }

        public int UserId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public bool DarkMode { get; set; }
        public bool IsDeleted { get; set; }

        public virtual ICollection<Category> Categories { get; set; }
        public virtual ICollection<Token> Tokens { get; set; }
    }
}
