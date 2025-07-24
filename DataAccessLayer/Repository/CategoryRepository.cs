using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.IRepository
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly LoafNcattingDbContext _context;

        public CategoryRepository(LoafNcattingDbContext context)
        {
            _context = context;
        }

        public bool AddCategories(Category category)
        {
            _context.Categories.Add(category);
            return _context.SaveChanges() > 0;  
        }

        public bool DeleteCategories(int id)
        {
            _context.Categories.Remove(_context.Categories.Find(id));
            return _context.SaveChanges() > 0;
        }

        public List<Category> GetAllCategories()
        {
            return _context.Categories.ToList();    
        }

        public Category GetCategoryById(int id)
        {
            return _context.Categories.Find(id);
        }

        public bool UpdateCategories(Category category)
        {
            Category categoryUpdate = _context.Categories.Find(category.CategoryId);
            if (categoryUpdate == null)
            {
                return false;
            }
            categoryUpdate.Name = category.Name;
            categoryUpdate.Description = category.Description;
            return _context.SaveChanges() > 0;
        }
    }
}
