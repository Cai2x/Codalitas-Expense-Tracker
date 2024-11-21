using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.ServiceModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.Interfaces
{
    public interface IExpenseService
    {
        void AddExpense(ExpenseViewModel expenseModel, int userId);
        void DeleteExpense(int expenseId);
        void UpdateExpense(ExpenseViewModel expenseModel);
        List<ExpenseViewModel> RetrieveUserExpenses(int userId);
        ExpenseViewModel RetrieveExpense(int expenseId);
        ExpenseViewModel TotalRecord(int userId);
        List<ExpenseViewModel> DateFilter(DateTime startDate, DateTime endDate, int userId);
    }
}
