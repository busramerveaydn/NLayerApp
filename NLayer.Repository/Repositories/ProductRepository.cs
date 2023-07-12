using Microsoft.EntityFrameworkCore;
using NLayer.Core.Models;
using NLayer.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLayer.Repository.Repositories
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        public ProductRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<List<Product>> GetProductsWithCategory()
        {
            // include metodu ile ; Eager Loading yaptık yani, datayı çekerken kategorilerinde alınmasını istedik.

            // Lazy Loading; product a bağlı kategoriyi daha sonra çekersek 

            return await _context.Products.Include(p => p.Category).ToListAsync();
        }
    }
}
