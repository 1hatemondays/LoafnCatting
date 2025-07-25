using DataAccessLayer.IRepository;
using DataAccessLayer.Models;
using System.Collections.Generic;
using System.Linq;

namespace DataAccessLayer.Repository
{
    public class GenderRepository : IGenderRepository
    {
        private readonly LoafNcattingDbContext _context;
        public GenderRepository(LoafNcattingDbContext context)
        {
            _context = context;
        }

        public List<Gender> GetAllGenders()
        {
            return _context.Genders.ToList();
        }
    }
}