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
    public class RestaurantTableService : IRestaurantTableService
    {
        private readonly IRestaurantTableRepository _restaurantTableRepository; 
        public RestaurantTableService(IRestaurantTableRepository restaurantTableRepository)
        {
            _restaurantTableRepository = restaurantTableRepository;
        }

        public bool AddRestaurantTable(RestaurantTable table)
        {
            return _restaurantTableRepository.AddRestaurantTable(table);
        }

        public bool DeleteRestaurantTable(int id)
        {
            return _restaurantTableRepository.DeleteRestaurantTable(id);    
        }

        public List<RestaurantTable> GetAllRestaurantTables()
        {
            return _restaurantTableRepository.GetAllRestaurantTables(); 
        }

        public RestaurantTable GetRestaurantTableById(int id)
        {
            return _restaurantTableRepository.GetRestaurantTableById(id);   
        }

        public bool UpdateRestaurantTable(RestaurantTable table)
        {
            return _restaurantTableRepository.UpdateRestaurantTable(table); 
        }
    }
}
