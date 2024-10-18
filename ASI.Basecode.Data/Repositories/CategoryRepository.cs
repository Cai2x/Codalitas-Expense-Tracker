using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using Basecode.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Repositories
{
    public class CategoryRepository : BaseRepository, ICategoryRepository
    {
        public CategoryRepository(IUnitOfWork unitOfWork): base(unitOfWork) { }
        public IQueryable<Category> RetrieveCategory()
        {
            return this.GetDbSet<Category>();
        }
        public void AddCategory(Category category)
        {
            this.GetDbSet<Category>().Add(category);
            UnitOfWork.SaveChanges();
        }
        public void DeleteCategory(Category category)
        {
            this.GetDbSet<Category>().Remove(category);
            UnitOfWork.SaveChanges();
        }
        public void UpdateCategory(Category category)
        {
            this.GetDbSet<Category>().Update(category);
            UnitOfWork.SaveChanges();
        }
    }
}
