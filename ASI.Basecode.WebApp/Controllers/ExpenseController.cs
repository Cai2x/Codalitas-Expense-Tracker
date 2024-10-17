﻿using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.Manager;
using ASI.Basecode.Services.ServiceModels;
using ASI.Basecode.WebApp.Authentication;
using ASI.Basecode.WebApp.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ASI.Basecode.WebApp.Controllers
{
    public class ExpenseController : ControllerBase<ExpenseController>
    {
        private readonly IExpenseService _expenseService;
        public ExpenseController(
                            IHttpContextAccessor httpContextAccessor,
                            ILoggerFactory loggerFactory,
                            IConfiguration configuration,
                            IMapper mapper,
                            IExpenseService expenseService) : base(httpContextAccessor, loggerFactory, configuration, mapper)
        {
           _expenseService = expenseService;
        }

        #region Get Methods
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var expense = _expenseService.RetrieveExpense(id);
            return View(expense);
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var expense = _expenseService.RetrieveExpense(id);
            return View(expense);
        }

        #endregion

        #region POST Methods
        [HttpPost]
        public IActionResult Create(ExpenseViewModel expense)
        {
            _expenseService.AddExpense(expense, int.Parse(UserId));
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Edit(ExpenseViewModel expense)
        {
            _expenseService.UpdateExpense(expense);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult PostDelete(int Id)
        {
            _expenseService.DeleteExpense(Id);
            return RedirectToAction("Index");
        }
#endregion

public IActionResult Index()
        {
            return View();
        }
    }
}
