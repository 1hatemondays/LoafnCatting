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
        public User? GetUserByEmailAndPassword(string email, string password)
        {
            return _userRepository.GetUserByEmailAndPassword(email, password);
        }
    }
}
