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
        public IActionResult Display()
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
                return Ok(category);

            }
            catch
            {
                return BadRequest(category);
            }
            
            //return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Edit(CategoryViewModel category)
        {
            _categoryService.UpdateCategory(category);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult PostDelete(int Id)
        {
            _categoryService.DeleteCategory(Id);
            return RedirectToAction("Index");
        }
        #endregion

        public IActionResult Index()
        {
            return Display();
        }
    }
}
