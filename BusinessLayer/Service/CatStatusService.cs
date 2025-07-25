using BusinessLayer.IService;
using DataAccessLayer.IRepository;
using DataAccessLayer.Models;
using System.Collections.Generic;

namespace BusinessLayer.Service
{
    public class CatStatusService : ICatStatusService
    {
        private readonly ICatStatusRepository _catStatusRepository;
        public CatStatusService(ICatStatusRepository catStatusRepository)
        {
            _catStatusRepository = catStatusRepository;
        }

        public List<CatStatus> GetAllCatStatuses()
        {
            return _catStatusRepository.GetAllCatStatuses();
        }
    }
}