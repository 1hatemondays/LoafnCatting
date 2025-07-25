using DataAccessLayer.IRepository;
using DataAccessLayer.Models;
using System.Collections.Generic;
using System.Linq;

namespace DataAccessLayer.Repository
{
    public class CatStatusRepository : ICatStatusRepository
    {
        private readonly LoafNcattingDbContext _context;
        public CatStatusRepository(LoafNcattingDbContext context)
        {
            _context = context;
        }

        public List<CatStatus> GetAllCatStatuses()
        {
            return _context.CatStatuses.ToList();
        }
    }
}