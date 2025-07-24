using DataAccessLayer.IRepository;
using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repository
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
            Cat cat = _context.Cats.FirstOrDefault(c=>c.CatId == id);
            if(cat == null)
            {
                return false;
            }
            _context.Cats.Remove(cat);
            return _context.SaveChanges() > 0;
        }

        public List<Cat> GetAllCats()
        {
            return _context.Cats.ToList();
        }

        public Cat GetCatById(int id)
        {
            return _context.Cats.FirstOrDefault(c=>c.CatId==id);
        }

        public bool UpdateCat(Cat cat)
        {
            Cat catUpdate = _context.Cats.FirstOrDefault(c => c.CatId == cat.CatId);
            if(catUpdate == null)
            {
                return false;
            }
            catUpdate.Name = cat.Name;
            catUpdate.Age = cat.Age;
            catUpdate.GenderId = cat.GenderId;
            catUpdate.Breed = cat.Breed;
            catUpdate.FriendlinessRating=cat.FriendlinessRating;
            catUpdate.Picture = cat.Picture;
            catUpdate.Description = cat.Description;
            catUpdate.StatusId = cat.StatusId;
            return _context.SaveChanges() > 0;
        }
    }
}
