using DataAccessLayer.Models;
using System.Collections.Generic;

namespace DataAccessLayer.IRepository
{
    public interface IGenderRepository
    {
        List<Gender> GetAllGenders();
    }
}