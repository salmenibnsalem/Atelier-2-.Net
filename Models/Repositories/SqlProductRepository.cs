using Atelier2.Models;
using Atelier2.Models.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Atelier2.Models.Repositories
{
    public class SqlProductRepository : IRepository<Product>
    {
        private readonly AppDbContext context;

        public SqlProductRepository(AppDbContext context) => this.context = context;

        public IEnumerable<Product> GetAll()
        {
            return context.Products.ToList();
        }

        public Product? Get(int id)
        {
            return context.Products.Find(id); 
        }

        public Product Add(Product product)
        {
            context.Products.Add(product);
            context.SaveChanges();
            return product;
        }

        public Product Update(Product product)
        {
            var existingProduct = context.Products.Attach(product);
            existingProduct.State = EntityState.Modified;
            context.SaveChanges();
            return product;
        }

        public Product? Delete(int id)
        {
            var product = context.Products.Find(id);
            if (product != null)
            {
                context.Products.Remove(product);
                context.SaveChanges();
            }
            return product;
        }
    }
}
