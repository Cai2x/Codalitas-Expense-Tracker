using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.Manager;
using ASI.Basecode.Services.ServiceModels;
using ASI.Basecode.Services.Services;
using ASI.Basecode.WebApp.Authentication;
using ASI.Basecode.WebApp.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ASI.Basecode.WebApp.Controllers
{
    public class ExpenseController : ControllerBase<ExpenseController>
    {
        private readonly IExpenseService _expenseService;
        private readonly ICategoryService _categoryService;
        public ExpenseController(
                            IHttpContextAccessor httpContextAccessor,
                            ILoggerFactory loggerFactory,
                            IConfiguration configuration,
                            IMapper mapper,
                            IExpenseService expenseService,
                            ICategoryService categoryService) : base(httpContextAccessor, loggerFactory, configuration, mapper)
        {
            _expenseService = expenseService;
            _categoryService = categoryService;
        }

        #region Get Methods
        [HttpGet]
        public IActionResult Display(int page = 1)
        {
            try
            {
                int pageSize = 7; // items per page for pagination

                var data = _expenseService.RetrieveUserExpenses(int.Parse(UserId));
                if (data == null || !data.Any())
                {
                    return View(null);  // Return an empty list to avoid null reference
                }

                var paginatedExpenses = data
                   .Skip((page - 1) * pageSize)
                   .Take(pageSize)
                   .ToList();

                // Calculate total pages
                var totalExpenses = data.Count;
                var totalPages = (int)Math.Ceiling(totalExpenses / (double)pageSize);

                ViewBag.CurrentPage = page;
                ViewBag.TotalPages = totalPages;

                // Return the view with the retrieved data
                return View("Index", paginatedExpenses);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public IActionResult SearchFilter(int page, string expenseName, string sortType, int? filterCategory, int pageSize = 7)
        {
            try
            {
                // Retrieve the user's expenses (or modify the logic as per your business rules)
                var data = _expenseService.SearchFilterExpense(expenseName, sortType, filterCategory, int.Parse(UserId));

                // If no data exists, return an empty response
                if (data == null || !data.Any())
                {
                    return Ok(new { success = false, data = data, totalPages = 0, currentPage = page });
                }

                // Apply pagination logic
                var totalItems = data.Count();
                var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
                var paginatedData = data.Skip((page - 1) * pageSize).Take(pageSize).ToList();

                // Return response with success status, data, and pagination info
                return Ok(new
                {
                    success = true,
                    data = paginatedData,
                    totalPages = totalPages,
                    currentPage = page
                });
            }
            catch (Exception ex)
            {
                // Return bad request in case of error
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult Category()
        {
            try
            {
                var categories = _categoryService.RetrieveUserCategory(int.Parse(UserId));

                if (categories == null || !categories.Any())
                {
                    return Ok(new { success = false, data = categories });
                }

                return Ok(new { success = true, data = categories });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            try
            {
                var expense = _expenseService.RetrieveExpense(id);
                //return View(expense);
                return Ok(expense);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            try
            {
                var expense = _expenseService.RetrieveExpense(id);
                TempData["SuccessMessage"] = "Expense Deleted successfully!";
                return Ok(expense);
                //return View(expense);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion


        #region POST Methods
        [AllowAnonymous]
        [HttpPost]
        public IActionResult Create(ExpenseViewModel expense, int currentPage = 1)
        {
            try
            {
                _expenseService.AddExpense(expense, int.Parse(UserId));
                TempData["SuccessMessage"] = "Expense added successfully!";


                var data = _expenseService.RetrieveUserExpenses(int.Parse(UserId));
                int totalExpenses = data.Count;
                int pageSize = 7;  // items per page

                int totalPages = (int)Math.Ceiling(totalExpenses / (double)pageSize);

                // Redirect to last page if the current page is less than total pages
                if (currentPage < totalPages)
                {
                    return RedirectToAction("Display", new { page = totalPages });
                }
                else
                {
                    return RedirectToAction("Display", new { page = currentPage });
                }
            }
            catch
            {
                TempData["ErrorMessage"] = "An error occurred while adding the expense.";
                return RedirectToAction("Display", new { page = currentPage });
            }
            //return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Edit(ExpenseViewModel expense, int currentPage)
        {
            try
            {
                _expenseService.UpdateExpense(expense);
                return Json(new { success = true, currentPage = currentPage });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public IActionResult PostDelete(int Id, int currentPage)
        {
            try
            {
                _expenseService.DeleteExpense(Id);
                return Json(new { success = true, currentPage = currentPage });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion

        public IActionResult Index()
        {
            return Display();
        }
    }
}