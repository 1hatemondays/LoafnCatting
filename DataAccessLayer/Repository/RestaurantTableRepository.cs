using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.IRepository
{
    public class RestaurantTableRepository : IRestaurantTableRepository
    {
        private readonly LoafNcattingDbContext _context;
        public RestaurantTableRepository(LoafNcattingDbContext context)
        {
            _context = context;
        }
        public bool AddRestaurantTable(RestaurantTable table)
        {
            throw new NotImplementedException();
        }

        public bool DeleteRestaurantTable(int id)
        {
            throw new NotImplementedException();
        }

        public List<RestaurantTable> GetAllRestaurantTables()
        {
            throw new NotImplementedException();
        }

        public RestaurantTable GetRestaurantTableById(int id)
        {
            throw new NotImplementedException();
        }

        public bool UpdateRestaurantTable(RestaurantTable table)
        {
            throw new NotImplementedException();
        }
    }
}
