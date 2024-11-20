using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.WebApp.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Linq;
using System;
using ASI.Basecode.Services.ServiceModels;
using ASI.Basecode.WebApp.Models;

namespace ASI.Basecode.WebApp.Controllers
{
    /// <summary>
    /// Home Controller
    /// </summary>
    public class HomeController : ControllerBase<HomeController>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="httpContextAccessor"></param>
        /// <param name="loggerFactory"></param>
        /// <param name="configuration"></param>
        /// <param name="localizer"></param>
        /// <param name="mapper"></param>
        
        private readonly IExpenseService _expenseService;
        private readonly ICategoryService _categoryService;
        public HomeController(
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

        /// <summary>
        /// Returns Home View.
        /// </summary>
        /// <returns> Home View </returns>
        public IActionResult Index()
        {
            var data = _expenseService.RetrieveUserExpenses(int.Parse(UserId));
            var total = _expenseService.TotalRecord(int.Parse(UserId));

            var dashboardModel = new DashboardViewModel
            {
                Expenses = data,
                TotalExpense = total.TotalExpense,
                TotalTransaction = total.TotalTransaction,
            };

            return View(dashboardModel);
        }

        public IActionResult DataParse()
        {
            try
            {
                var categories = _categoryService.RetrieveUserCategory(int.Parse(UserId));

                if (categories == null || !categories.Any())
                {
                    return Ok(new { success = false, data = categories });
                }

                var expenses = _expenseService.RetrieveUserExpenses(int.Parse(UserId));

                if (expenses == null || !expenses.Any())
                {
                    return Ok(new { success = false, data_category = categories, data_expense = expenses });
                }

                return Ok(new { success = true, data_category = categories, data_expense = expenses });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        public IActionResult DataParseFilterByDate(DateTime start, DateTime end)
        {
            try
            {
                if(start > end)
                {
                    TempData["ErrorMessage"] = "Start Date should be less than End Date";
                    return Ok(new { success = false });
                }

                var categories = _categoryService.RetrieveUserCategory(int.Parse(UserId));

                if (categories == null || !categories.Any())
                {
                    return Ok(new { success = false, data = categories });
                }

                var expenses = _expenseService.DateFilter(start, end, int.Parse(UserId));

                if (expenses == null || !expenses.Any())
                {
                    return Ok(new { success = false, data_category = categories, data_expense = expenses });
                }

                return Ok(new { success = true, data_category = categories, data_expense = expenses });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
