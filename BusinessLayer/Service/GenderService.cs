using BusinessLayer.IService;
using DataAccessLayer.IRepository;
using DataAccessLayer.Models;
using System.Collections.Generic;

namespace BusinessLayer.Service
{
    public class GenderService : IGenderService
    {
        private readonly IGenderRepository _genderRepository;
        public GenderService(IGenderRepository genderRepository)
        {
            _genderRepository = genderRepository;
        }

        public List<Gender> GetAllGenders()
        {
            return _genderRepository.GetAllGenders();
        }
    }
}