using ASI.Basecode.Services.ServiceModels;
using System;
using System.Collections.Generic;

namespace ASI.Basecode.WebApp.Models
{
    public class DashboardViewModel
    {
        public IEnumerable<ExpenseViewModel> Expenses { get; set; }
        public double TotalExpense { get; set; }
        public int TotalTransaction { get; set; }
    }
}
