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
            _context.Cats.Add(cat); 
            return _context.SaveChanges() > 0;
        }

        public bool DeleteCat(int id)
        {
            _context.Cats.Remove(_context.Cats.Find(id));
            return _context.SaveChanges() > 0;
        }

        public List<Cat> GetAllCats()
        {
            return _context.Cats.ToList();
        }

        public Cat GetCatById(int id)
        {
            return _context.Cats.Find(id);
        }

        public bool UpdateCat(Cat cat)
        {
            _context.Cats.Update(cat);
            return _context.SaveChanges() > 0;
        }
    }
}
