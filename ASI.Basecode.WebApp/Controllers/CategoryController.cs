﻿using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using ASI.Basecode.Services.Services;
using ASI.Basecode.WebApp.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Linq;

namespace ASI.Basecode.WebApp.Controllers
{
    public class CategoryController : ControllerBase<CategoryController>
    {
        private readonly ICategoryService _categoryService;
        private const int PageSize = 7;  // Max 7 data per table
        public CategoryController(
            IHttpContextAccessor httpContextAccessor, 
            ILoggerFactory loggerFactory, 
            IConfiguration configuration, 
            IMapper mapper,
            ICategoryService categoryService) : base(httpContextAccessor, loggerFactory, configuration, mapper)
        {
            _categoryService = categoryService;
        }


        #region Get Methods
        [HttpGet]
        public IActionResult Display(int page = 1)
        {
            try
            {
                var data = _categoryService.RetrieveUserCategory(int.Parse(UserId));
                //return Ok(data);
                return View(data);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            try
            {
                var category = _categoryService.RetrieveCategory(id);
                return Ok(category);
                //return View(category);
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
                var category = _categoryService.RetrieveCategory(id);               
                return Ok(category);              
                //return View(category);
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
        public IActionResult Create(CategoryViewModel category)
        {
            try
            {
                _categoryService.AddCategory(category, int.Parse(UserId));
                TempData["SuccessMessage"] = "Category added successfully!";
                return RedirectToAction("Index");
                
            }

            catch
            {
                TempData["ErrorMessage"] = "Category Name Already Exists!";
                return RedirectToAction("Index");
            }
           
        }

        [HttpPost]
        public IActionResult Edit(CategoryViewModel category)
        {
            _categoryService.UpdateCategory(category);
            TempData["SuccessMessage"] = "Category Updated successfully!";
            TempData.Keep("SuccessMessage");
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult PostDelete(int Id)
        {
            _categoryService.DeleteCategory(Id);
            TempData["SuccessMessage"] = "Category deleted successfully!";          
            return RedirectToAction("Index");
        }
        #endregion

        public IActionResult Index()
        {
            return Display();
        }
    }
}
