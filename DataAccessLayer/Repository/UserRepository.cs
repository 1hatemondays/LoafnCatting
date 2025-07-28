using DataAccessLayer.IRepository;
using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly LoafNcattingDbContext _context;
        public UserRepository(LoafNcattingDbContext context)
        {
            _context = context;
        }

        public bool AddUser(User user)
        {
            _context.Users.Add(user);
            return _context.SaveChanges() > 0;
        }

        public bool DeleteUser(int id)
        {
            User user=_context.Users.FirstOrDefault(u=>u.UserId == id);
            if (user == null)
            {
                return false;
            }
            _context.Remove(user);
            return _context.SaveChanges() > 0;
        }

        public List<User> GetAllUsers()
        {
            return _context.Users.ToList();
        }

        public List<User> GetAllUsersByRoleId(int roleId)
        {
            return _context.Users
                .Where(u => u.RoleId == roleId)
                .Include(u => u.Role)
                .ToList();  
        }

        public User? GetUserByEmailAndPassword(string email, string password)
        {
            return _context.Users
                .Include(u => u.Role)
                .FirstOrDefault(u => u.Email == email && u.Password == password);
        }

        public User GetUserById(int id)
        {
            return _context.Users.FirstOrDefault(u => u.UserId == id);
        }

        public bool UpdateUser(User user)
        {
            User userUpdate = _context.Users.FirstOrDefault(u=>u.UserId==user.UserId);
            if (userUpdate == null)
            {
                System.Diagnostics.Debug.WriteLine("User not found for update operation.");
                return false;
            }
            userUpdate.Name=user.Name;
            userUpdate.PhoneNumber=user.PhoneNumber;
            userUpdate.Email = user.Email;
            userUpdate.Password = user.Password;
            userUpdate.RoleId = user.RoleId;
            return _context.SaveChanges() > 0;
        }
    }
}
