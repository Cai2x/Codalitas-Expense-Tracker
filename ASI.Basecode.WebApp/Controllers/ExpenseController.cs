﻿using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.Manager;
using ASI.Basecode.Services.ServiceModels;
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
        public IActionResult Display()
        {
            try
            {
                var data = _expenseService.RetrieveUserExpenses(int.Parse(UserId));

                if (data is null)
                {
                    // Return an empty list to avoid null reference
                    return View(null);
                }

                // Return the view with the retrieved data
                return View(data);
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
            catch(Exception ex)
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
                return Ok(expense);
                //return View(expense);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #endregion


        #region POST Methods
        [AllowAnonymous]
        [HttpPost]
        public IActionResult Create(ExpenseViewModel expense)
        {
            try
            {
                _expenseService.AddExpense(expense, int.Parse(UserId));
                return Ok(expense);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
            //return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Edit(ExpenseViewModel expense)
        {
            try
            {
                _expenseService.UpdateExpense(expense);
                return Ok(expense);
                //return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public IActionResult PostDelete(int Id)
        {
            try
            {
                _expenseService.DeleteExpense(Id);
                return Ok(Id);
                //return RedirectToAction("Index");
            }
            catch(Exception ex)
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
