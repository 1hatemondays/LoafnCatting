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
    public class CatService : ICatService
    {
        private readonly ICatRepository _catRepository;
        
        public CatService(ICatRepository catRepository)
        {
            _catRepository = catRepository;
        }

        public bool AddCat(Cat cat)
        {
            return _catRepository.AddCat(cat);  
        }

        public bool DeleteCat(int id)
        {
            return _catRepository.DeleteCat(id);
        }

        public List<Cat> GetAllCats()
        {
            return _catRepository.GetAllCats();
        }

        public Cat GetCatById(int id)
        {
            return _catRepository.GetCatById(id);
        }

        public List<Cat> GetCatsByGenderId(int genderId)
        {
            return _catRepository.GetCatsByGenderId(genderId);
        }

        public List<Cat> GetCatsByStatusId(int statusId)
        {
            return _catRepository.GetCatsByStatusId(statusId);  
        }

        public bool UpdateCat(Cat cat)
        {
            return _catRepository.UpdateCat(cat);   
        }
    }
}
