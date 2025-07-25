using DataAccessLayer.Models;
using System.Collections.Generic;

namespace DataAccessLayer.IRepository
{
    public interface ICatStatusRepository
    {
        List<CatStatus> GetAllCatStatuses();
    }
}