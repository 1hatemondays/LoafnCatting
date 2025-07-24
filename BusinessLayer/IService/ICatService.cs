using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.IService
{
    public interface ICatService
    {
        public List<Cat> GetAllCats();
        public Cat GetCatById(int id);
        public bool AddCat(Cat cat);
        public bool UpdateCat(Cat cat);
        public bool DeleteCat(int id);
        public List<Cat> GetCatsByStatusId(int statusId);
        public List<Cat> GetCatsByGenderId(int genderId);

    }
}
