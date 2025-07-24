using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.IRepository
{
    public interface ICategoryRepository
    {
        public List<Category> GetAllCategories();
        public Category GetCategoryById(int id);
        public bool AddCategories(Category category);
        public bool UpdateCategories(Category category);
        public bool DeleteCategories(int id);
    }
}
