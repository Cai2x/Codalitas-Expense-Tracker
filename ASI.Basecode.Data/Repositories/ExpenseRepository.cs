using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using Basecode.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Repositories
{
    public class ExpenseRepository : BaseRepository, IExpenseRepository
    {
        public ExpenseRepository(IUnitOfWork unitOfWork): base(unitOfWork) { }

        public IQueryable<Expense> RetrieveExpenses()
        {
            return this.GetDbSet<Expense>();
        }
        public void AddExpense(Expense expense)
        {
            this.GetDbSet<Expense>().Add(expense);
            UnitOfWork.SaveChanges();
        }
        public void DeleteExpense(Expense expense)
        {
            this.GetDbSet<Expense>().Remove(expense);
            UnitOfWork.SaveChanges();
        }
        public void UpdateExpense(Expense expense)
        {
            this.GetDbSet<Expense>().Update(expense);
            UnitOfWork.SaveChanges();
        }
    }
}
