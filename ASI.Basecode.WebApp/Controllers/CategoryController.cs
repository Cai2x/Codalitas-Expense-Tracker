using ASI.Basecode.Services.Interfaces;
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
using System.Drawing.Printing;

namespace ASI.Basecode.WebApp.Controllers
{
    public class CategoryController : ControllerBase<CategoryController>
    {
        private readonly ICategoryService _categoryService;
        
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
                int userId = int.Parse(UserId);
                int pageSize = 7; // items per page for pagination

                var data = _categoryService.RetrieveUserCategory(int.Parse(UserId));
                //return Ok(data);

                var paginatedCategories = data
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

                // Calculate total pages
                var totalCategories = data.Count;
                var totalPages = (int)Math.Ceiling(totalCategories / (double)pageSize);

                ViewBag.CurrentPage = page;
                ViewBag.TotalPages = totalPages;

                return View("Index", paginatedCategories);
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
        public IActionResult Create(CategoryViewModel category, int currentPage = 1)
        {
            try
            {
                _categoryService.AddCategory(category, int.Parse(UserId));
                TempData["SuccessMessage"] = "Category added successfully!";
                

                var data = _categoryService.RetrieveUserCategory(int.Parse(UserId));
                int totalCategories = data.Count;
                int pageSize = 7;  


                
                int totalPages = (int)Math.Ceiling(totalCategories / (double)pageSize);
               
                if (currentPage < totalPages)
                {
                    // Redirect to the last page
                    return RedirectToAction("Display", new { page = totalPages });
                }
                else
                {
                    
                    return RedirectToAction("Display", new { page = currentPage });
                }
            

            }

            catch
            {
                TempData["ErrorMessage"] = "Category Name Already Exists!";
                return RedirectToAction("Display", new { page = currentPage });
            }
           
        }

        [HttpPost]
        public IActionResult Edit(CategoryViewModel category, int currentPage)
        {
            _categoryService.UpdateCategory(category);
            TempData["SuccessMessage"] = "Category Updated successfully!";
            
            return RedirectToAction("Display", new { page = currentPage });
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
