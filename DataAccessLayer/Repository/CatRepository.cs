using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.IRepository
{
    public class CatRepository : ICatRepository
    {
        private readonly LoafNcattingDbContext _context;

        public CatRepository(LoafNcattingDbContext context)
        {
            _context = context;
        }
        public bool AddCat(Cat cat)
        {
            throw new NotImplementedException();
        }

        public bool DeleteCat(int id)
        {
            throw new NotImplementedException();
        }

        public List<Cat> GetAllCats()
        {
            throw new NotImplementedException();
        }

        public Cat GetCatById(int id)
        {
            throw new NotImplementedException();
        }

        public bool UpdateCat(Cat cat)
        {
            throw new NotImplementedException();
        }
    }
}
