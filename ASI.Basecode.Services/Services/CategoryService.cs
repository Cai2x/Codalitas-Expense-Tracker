using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Data.Repositories;
using ASI.Basecode.Services.ServiceModels;
using System.IO;
using AutoMapper;

namespace ASI.Basecode.Services.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public CategoryService(ICategoryRepository categoryRepository, IMapper mapper) 
        { 
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public List<CategoryViewModel> RetrieveUserCategory(int userId)
        {
            var retrievedData = _categoryRepository.RetrieveCategory().Where(x => x.CreatedBy == userId)
                .Select(x => new CategoryViewModel
                {
                    CategoryId = x.CategoryId,
                    Name = x.Name,
                    CategoryDateCreated = x.DateCreated,
                    Description = x.Description,
                }).ToList();

            //Put this on after ni Jule para sa Foreign Key Application
            /*var retrievedData = _expenseRepository.RetrieveExpenses().Where(x=>x.UserId == userId).Join(_categoryRepository.RetrieveCategories(), 
                expense => expense.CategoryId,
                category = > category.CategoryId,
                (expense,category) => new ExpenseViewModel
                {
                    ExpenseId = expense.ExpenseId,
                    Title = expense.Title,
                    Amount = expense.Amount,
                    Description = expense.Description,
                    CategoryId = category.CategoryId,
                    CategoryName = category.CategoryName,
                }).ToList();*/

            return retrievedData;
        }

        public CategoryViewModel RetrieveCategory(int categoryId)
        {
            var category = _categoryRepository.RetrieveCategory().Where(x => x.CategoryId == categoryId)
                .Select(e => new CategoryViewModel
                {
                    CategoryId = e.CategoryId,
                    Name = e.Name,
                    CategoryDateCreated = e.DateCreated,
                    Description = e.Description,
                }).FirstOrDefault();

            return category;
        }

        public void AddCategory(CategoryViewModel categoryModel, int userId)
        {
            var category = new Category();
            _mapper.Map(categoryModel, category);
            category.CreatedBy = userId;
            category.DateCreated = DateTime.Now;
            category.DateUpdated = DateTime.Now;
            try
            {
                _categoryRepository.AddCategory(category);
            }

            catch (Exception ex)
            {
                throw new InvalidDataException(Resources.Messages.Errors.ServerError);
            }
        }

        public void DeleteExpense(int categoryId)
        {
            var category = _categoryRepository.RetrieveCategory().Where(x => x.CategoryId == categoryId).FirstOrDefault();
            if (category != null)
            {
                _categoryRepository.DeleteCategory(category);
            }
        }

        public void UpdateExpense(CategoryViewModel categoryModel)
        {
            var category = new Category();
            _mapper.Map(categoryModel, category);
            category.DateUpdated = DateTime.Now;
            try
            {
                _categoryRepository.UpdateCategory(category);
            }

            catch (Exception)
            {
                throw new InvalidDataException(Resources.Messages.Errors.ServerError);
            }
        }
    }
}
