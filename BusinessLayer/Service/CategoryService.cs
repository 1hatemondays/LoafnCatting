using BusinessLayer.IService;
using DataAccessLayer.IRepository;
using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Service
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public bool AddCategories(Category category)
        {
            return _categoryRepository.AddCategories(category);
        }

        public bool DeleteCategories(int id)
        {
            return _categoryRepository.DeleteCategories(id);
        }

        public List<Category> GetAllCategories()
        {
            return _categoryRepository.GetAllCategories();
        }

        public Category GetCategoryById(int id)
        {
            return _categoryRepository.GetCategoryById(id); 
        }

        public bool UpdateCategories(Category category)
        {
            return _categoryRepository.UpdateCategories(category);  
        }
    }
}
