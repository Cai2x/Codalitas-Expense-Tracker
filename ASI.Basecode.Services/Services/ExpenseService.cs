using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Data.Repositories;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.Manager;
using ASI.Basecode.Services.ServiceModels;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.Services
{
    public class ExpenseService : IExpenseService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IExpenseRepository _expenseRepository;
        private readonly IMapper _mapper;
        public ExpenseService(IExpenseRepository expenseRepository, ICategoryRepository categoryRepository, IMapper mapper) {
            _categoryRepository = categoryRepository;
            _expenseRepository = expenseRepository;
            _mapper = mapper;
        }

        public List<ExpenseViewModel> RetrieveUserExpenses(int userId)
        {
            var retrievedData = _expenseRepository.RetrieveExpenses().Where(x=>x.UserId == userId).Join(_categoryRepository.RetrieveCategory(), 
                expense => expense.CategoryId,
                category => category.CategoryId,
                (expense,category) => new ExpenseViewModel
                {
                    ExpenseId = expense.ExpenseId,
                    Title = expense.Title,
                    Amount = expense.Amount,
                    Description = expense.Description,
                    CategoryId = category.CategoryId,
                    CategoryName = category.Name,
                }).ToList();

            return retrievedData;
        }

        public ExpenseViewModel RetrieveExpense(int expenseId)
        {
            var expense = _expenseRepository.RetrieveExpenses().Where(x => x.ExpenseId == expenseId)
                .Select(e => new ExpenseViewModel
                {
                    ExpenseId = e.ExpenseId,
                    Title = e.Title,
                    Amount = e.Amount,
                    Description = e.Description,
                    CategoryId = e.CategoryId,
                }).FirstOrDefault();

            return expense;
        }

        public void AddExpense(ExpenseViewModel expenseModel, int userId)
        {
            var expense = new Expense();
            _mapper.Map(expenseModel, expense);
            expense.UserId = userId;
            expense.DateCreated = DateTime.Now;
            expense.DateUpdated = DateTime.Now;
            try
            {
                _expenseRepository.AddExpense(expense);
            }

            catch (Exception)
            {
                throw new InvalidDataException(Resources.Messages.Errors.UserExists);
            }
        }

        public void DeleteExpense(int expenseId)
        {
            var expense = _expenseRepository.RetrieveExpenses().Where(x=>x.ExpenseId == expenseId).FirstOrDefault();
            if(expense != null)
            {
                _expenseRepository.DeleteExpense(expense);
            }
        }

        public void UpdateExpense(ExpenseViewModel expenseModel)
        {
            var expense = new Expense();
            _mapper.Map(expenseModel, expense);
            expense.DateUpdated = DateTime.Now;
            try
            {
                _expenseRepository.UpdateExpense(expense);
            }

            catch (Exception)
            {
                throw new InvalidDataException(Resources.Messages.Errors.ServerError);
            }
        }
    }
}
