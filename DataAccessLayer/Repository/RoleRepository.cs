using DataAccessLayer.IRepository;
using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repository
{
    public class RoleRepository : IRoleRepository
    {
        private readonly LoafNcattingDbContext _context;
        public RoleRepository(LoafNcattingDbContext context)
        {
            _context = context;
        }
        public List<Role> GetAllRoles()
        {
            return _context.Roles.ToList();
        }
    }
}
