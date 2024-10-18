using ASI.Basecode.Services.ServiceModels;
using ASI.Basecode.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.Interfaces
{
    public interface ICategoryService
    {

        void AddCategory(CategoryViewModel categoryModel, int userId);
        void DeleteCategory(int categoryId);
        void UpdateCategory(CategoryViewModel categoryModel);
        List<CategoryViewModel> RetrieveUserCategory(int userId);
        CategoryViewModel RetrieveCategory(int categoryId);
    }
}
