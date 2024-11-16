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
            var retrievedData = _expenseRepository.RetrieveExpenses().Where(x=>x.UserId == userId && x.IsDeleted == false)
                .Join(_categoryRepository.RetrieveCategory().Where(c => c.IsDeleted == false), 
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
                    Status = expense.Status,
                    ExpenseDateCreated = expense.DateCreated,
                    Date = expense.Date
                }).ToList();

            return retrievedData;
        }

        public ExpenseViewModel RetrieveExpense(int expenseId)
        {
            var expense = _expenseRepository.RetrieveExpenses().Where(x => x.ExpenseId == expenseId)
                .Join(_categoryRepository.RetrieveCategory().Where(c => c.IsDeleted == false),
                expense => expense.CategoryId,
                category => category.CategoryId,
                (expense, category) => new ExpenseViewModel
                {
                    ExpenseId = expense.ExpenseId,
                    Title = expense.Title,
                    Amount = expense.Amount,
                    Description = expense.Description,
                    CategoryId = category.CategoryId,
                    CategoryName = category.Name,
                    Status = expense.Status,
                    ExpenseDateCreated = expense.DateCreated,
                    Date = expense.Date
                }).FirstOrDefault();

            if(expense is null)
            {
                throw new InvalidDataException(Resources.Messages.Errors.DataNotFound);
            }

            return expense;
        }

        public void AddExpense(ExpenseViewModel expenseModel, int userId)
        {
            var categoryExist = _categoryRepository.RetrieveCategory().Any(x => x.CategoryId == expenseModel.CategoryId);
            if(categoryExist is false)
            {
                throw new InvalidDataException(Resources.Messages.Errors.DataNotFound);
            }

            var expense = new Expense();
            _mapper.Map(expenseModel, expense);
            expense.UserId = userId;
            expense.DateCreated = DateTime.Now;
            expense.DateUpdated = DateTime.Now;

            try
            {
               _expenseRepository.AddExpense(expense);
            }

            catch (Exception ex)
            {
                //throw new InvalidDataException(Resources.Messages.Errors.ServerError);

                throw new InvalidDataException(ex.Message);
            }
        }

        public void DeleteExpense(int expenseId)
        {
            var expense = _expenseRepository.RetrieveExpenses().Where(x=>x.ExpenseId == expenseId).FirstOrDefault();
            expense.IsDeleted = true;
            try
            {
                if (expense != null)
                {
                    _expenseRepository.UpdateExpense(expense);
                }
            }
            catch (Exception)
            {
                throw new InvalidDataException(Resources.Messages.Errors.ServerError);
            }
        }

        public void UpdateExpense(ExpenseViewModel expenseModel)
        {
            var expense = _expenseRepository.RetrieveExpenses().Where(x => x.ExpenseId == expenseModel.ExpenseId).FirstOrDefault();

            if (expense is null)
            {
                throw new InvalidDataException(Resources.Messages.Errors.DataNotFound);
            }

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