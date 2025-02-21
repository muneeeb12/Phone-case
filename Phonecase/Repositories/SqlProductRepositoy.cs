using Microsoft.EntityFrameworkCore;
using Phonecase.Data;
using Phonecase.Models;

namespace Phonecase.Repositories
{
    public class SqlProductRepositoy: IProductRepository
    {
        private readonly PhoneCaseDbContext _context;

        public SqlProductRepositoy(PhoneCaseDbContext context)
        {
            _context = context;
        }

        public async Task<Product> CreateProductAsync(Product product)
        {
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            return (product);
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            var productList = await _context.Products
                .Include(p => p.Model)
                .Include(p => p.CaseManufacturer)
                .ToListAsync();

            return productList;
        }
    }
}
