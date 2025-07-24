using DataAccessLayer.IRepository;
using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repository
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
            _context.Add(table);
            return _context.SaveChanges() > 0;
        }

        public bool DeleteRestaurantTable(int id)
        {
            RestaurantTable table=_context.RestaurantTables.FirstOrDefault(r=>r.TableId==id);
            if (table==null)
            {
                return false;
            }
            _context.Remove(table);
            return _context.SaveChanges() > 0;
        }

        public List<RestaurantTable> GetAllRestaurantTables()
        {
            return _context.RestaurantTables.ToList();
        }

        public RestaurantTable GetRestaurantTableById(int id)
        {
            return _context.RestaurantTables.FirstOrDefault(r => r.TableId == id);
        }

        public List<RestaurantTable> GetRestaurantTableByStatusId(int statusId)
        {
            return _context.RestaurantTables
                .Where(r => r.TableStatusId == statusId)
                .ToList();  
        }

        public bool UpdateRestaurantTable(RestaurantTable table)
        {


            RestaurantTable tableUpdate = _context.RestaurantTables.FirstOrDefault(r => r.TableId == table.TableId);
            if (table == null)
            {
                return false;
            }
            tableUpdate.TableName = table.TableName;
            tableUpdate.TableStatusId = table.TableStatusId;
            return _context.SaveChanges() > 0;
        }
    }
}
