using ASI.Basecode.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Interfaces
{
    public interface IExpenseRepository
    {
        IQueryable<Expense> RetrieveExpenses();
        void AddExpense(Expense expense);
        void DeleteExpense(Expense expense);
        void UpdateExpense(Expense expense);
    }
}
