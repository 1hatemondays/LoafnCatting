using DataAccessLayer.IRepository;
using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repository
{
    public class UserRepository : IUserRepository
    {
        LoafNcattingDbContext _context;
        public UserRepository(LoafNcattingDbContext context)
        {
            _context = context;
        }
        public User? GetUserByEmailAndPassword(string email, string password)
        {
            return _context.Users.FirstOrDefault(u => u.Email == email && u.Password == password);
        }
    }
}
