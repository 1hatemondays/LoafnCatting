using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.IService
{
    public interface IRoleService
    {
        public List<Role> GetAllRoles();
    }
}
