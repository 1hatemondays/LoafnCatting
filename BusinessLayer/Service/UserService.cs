using BusinessLayer.IService;
using DataAccessLayer.IRepository;
using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public bool AddUser(User user)
        {
            return _userRepository.AddUser(user);
        }

        public bool DeleteUser(int id)
        {
            return _userRepository.DeleteUser(id);  
        }

        public List<User> GetAllUsers()
        {
            return _userRepository.GetAllUsers();
        }

        public List<User> GetAllUsersByRoleId(int roleId)
        {
            throw new NotImplementedException();
        }

        public User GetUserByEmailAndPassword(string email, string password)
        {
            return _userRepository.GetUserByEmailAndPassword(email, password);  
        }

        public User GetUserById(int id)
        {
            return _userRepository.GetUserById(id);
        }

        public bool UpdateUser(User user)
        {
            return _userRepository.UpdateUser(user);    
        }
    }
}
