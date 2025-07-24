using DataAccessLayer.IRepository;
using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repository
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
            Product product = _context.Products.FirstOrDefault(p => p.ProductId == id);
            if (product == null)
            {
                return false;
            }
            _context.Products.Remove(product);
            return _context.SaveChanges() > 0;
        }

        public List<Product> GetAllProducts()
        {
            return _context.Products.ToList();
        }

        public Product GetProductById(int id)
        {
            return _context.Products.FirstOrDefault(p => p.ProductId == id);
        }

        public bool UpdateProduct(Product product)
        {
            Product productUpdate = _context.Products.FirstOrDefault(p => p.ProductId == product.ProductId);
            if (productUpdate == null)
            {
                return false;
            }
            productUpdate.Name = product.Name;
            productUpdate.Description = product.Description;
            productUpdate.Price = product.Price;
            productUpdate.UnitInStock = product.UnitInStock;
            productUpdate.Picture = product.Picture;
            productUpdate.CategoryId = product.CategoryId;
            return _context.SaveChanges() > 0;
        }
    }
}
