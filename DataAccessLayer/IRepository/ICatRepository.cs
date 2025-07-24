using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.IRepository
{
    public interface ICatRepository
    {
        public List<Cat> GetAllCats();
        public Cat GetCatById(int id);
        public bool AddCat(Cat cat);
        public bool UpdateCat(Cat cat);
        public bool DeleteCat(int id);


    }
}
