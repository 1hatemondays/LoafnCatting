using DataAccessLayer.Models;
using System.Collections.Generic;

namespace BusinessLayer.IService
{
    public interface IGenderService
    {
        List<Gender> GetAllGenders();
    }
}