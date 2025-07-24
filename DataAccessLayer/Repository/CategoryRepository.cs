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
            throw new NotImplementedException();
        }

        public bool DeleteCategories(int id)
        {
            throw new NotImplementedException();
        }

        public List<Category> GetAllCategories()
        {
            throw new NotImplementedException();
        }

        public Category GetCategoryById(int id)
        {
            throw new NotImplementedException();
        }

        public bool UpdateCategories(Category category)
        {
            throw new NotImplementedException();
        }
    }
}
