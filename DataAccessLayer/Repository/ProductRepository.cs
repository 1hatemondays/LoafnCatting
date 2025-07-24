using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.IRepository
{
    public class ProductRepository : IProductRepository
    {
        private readonly LoafNcattingDbContext _context;
        public ProductRepository(LoafNcattingDbContext context)
        {
            _context = context;
        }
        public bool AddProduct(Product product)
        {
            _context.Products.Add(product);
            return _context.SaveChanges() > 0;
        }

        public bool DeleteProduct(int id)
        {
            _context.Products.Remove(_context.Products.Find(id));
            return _context.SaveChanges() > 0;
        }

        public List<Product> GetAllProducts()
        {
            return _context.Products.ToList();
        }

        public Product GetProductById(int id)
        {
            return _context.Products.Find(id);
        }

        public bool UpdateProduct(Product product)
        {
            _context.Products.Update(product);  
            return _context.SaveChanges() > 0;
        }
    }
}
