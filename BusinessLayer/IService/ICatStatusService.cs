using DataAccessLayer.Models;
using System.Collections.Generic;

namespace BusinessLayer.IService
{
    public interface ICatStatusService
    {
        List<CatStatus> GetAllCatStatuses();
    }
}