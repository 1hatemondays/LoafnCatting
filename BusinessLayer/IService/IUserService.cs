using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.IService
{
    public interface IUserService
    {
        public User GetUserByEmailAndPassword(string email, string password);
        public List<User> GetAllUsers();
        public User GetUserById(int id);
        public bool AddUser(User user);
        public bool UpdateUser(User user);
        public bool DeleteUser(int id);
    }
}
