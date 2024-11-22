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
                    Date = expense.Date,
        }).ToList();

            return retrievedData;
        }

        public ExpenseViewModel TotalRecord(int userId)
        {
            var expense = _expenseRepository.RetrieveExpenses().Where(x => x.UserId == userId && !x.IsDeleted && x.Status)
                 .Join(_categoryRepository.RetrieveCategory().Where(c => !c.IsDeleted),
                 expense => expense.CategoryId,
                 category => category.CategoryId,
                 (expense, category) => new ExpenseViewModel
                 {
                     ExpenseId = expense.ExpenseId,
                     Amount = expense.Amount,
                     CategoryName = category.Name
                 });

            double totalExpense = expense.Sum(e => e.Amount);
            int totalTransaction = expense.Count();

            return new ExpenseViewModel
            {
                TotalExpense = totalExpense,
                TotalTransaction = totalTransaction,
            };
        }

        public List<ExpenseViewModel> DateFilter(DateTime startDate, DateTime endDate, int userId)
        {
            var retrievedData = _expenseRepository.RetrieveExpenses()
               .Where(x => x.UserId == userId && !x.IsDeleted && x.Date <= endDate && x.Date >= startDate && x.Status)
               .Join(_categoryRepository.RetrieveCategory().Where(c => !c.IsDeleted),
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
                   Date = expense.Date,
               }).ToList();

            return retrievedData;
        }

        public List<ExpenseViewModel> SearchFilterExpense(string expenseName, string sort, int? filterId, int userId)
        {
            var data = _expenseRepository.RetrieveExpenses().Where(x => x.UserId == userId && !x.IsDeleted)
                .Join(_categoryRepository.RetrieveCategory().Where(c => !c.IsDeleted),
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
                    Date = expense.Date,
                }).ToList();

            //Filters the retrieved Expenses according to CategoryName
            if (expenseName != null)
            {
                data = data.Where(x => x.Title.ToLower() == expenseName.ToLower()).ToList();

                if(data is null)
                {
                    return data;
                }
            }
            
            if (filterId != null)
                data = data.Where(x => x.CategoryId == filterId).ToList();

            //Sorts the data gathered
            switch (sort)
            {
                case "amount_asc":
                    data = data.OrderBy(x => x.Amount).ToList();
                    break;
                case "amount_desc":
                    data = data.OrderByDescending(x => x.Amount).ToList();
                    break;
                case "date_asc":
                    data = data.OrderBy(x => x.Date).ToList();
                    break;
                case "date_desc":
                    data = data.OrderByDescending(x => x.Date).ToList();
                    break;
                default:
                    data = data.OrderBy(x => x.CategoryName).ToList();
                    break;
            }

            return data;
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