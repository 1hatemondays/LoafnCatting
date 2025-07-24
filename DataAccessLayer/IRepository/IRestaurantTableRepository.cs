using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.IRepository
{
    public interface IRestaurantTableRepository
    {
        public List<RestaurantTable> GetAllRestaurantTables();
        public RestaurantTable GetRestaurantTableById(int  id);
        public bool AddRestaurantTable(RestaurantTable table);
        public bool UpdateRestaurantTable(RestaurantTable table);
        public bool DeleteRestaurantTable(int id);
    }
}
