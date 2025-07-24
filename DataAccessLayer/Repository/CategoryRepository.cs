using DataAccessLayer.IRepository;
using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repository
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
            Category category= _context.Categories.FirstOrDefault(c => c.CategoryId==id);
            if (category == null)
            {
                return false;   
            }
            _context.Categories.Remove(category);
            return _context.SaveChanges() > 0;
        }

        public List<Category> GetAllCategories()
        {
            return _context.Categories.ToList();    
        }

        public Category GetCategoryById(int id)
        {
            return _context.Categories.FirstOrDefault(c=>c.CategoryId==id);
        }

        public bool UpdateCategories(Category category)
        {
            Category categoryUpdate = _context.Categories.FirstOrDefault(c => c.CategoryId == category.CategoryId);
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
