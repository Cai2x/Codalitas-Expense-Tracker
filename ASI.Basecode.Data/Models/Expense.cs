using System;
using System.Collections.Generic;

namespace ASI.Basecode.Data.Models
{
    public partial class Expense
    {
        public int ExpenseId { get; set; }
        public int CategoryId { get; set; }
        public int UserId { get; set; }
        public string Title { get; set; }
        public double Amount { get; set; }
        public DateTime DateCreated { get; set; }
        public string Description { get; set; }
        public bool Status { get; set; }
        public DateTime? DateUpdated { get; set; }
        public DateTime Date { get; set; }

        public virtual Category Category { get; set; }
        public virtual User User { get; set; }
    }
}
