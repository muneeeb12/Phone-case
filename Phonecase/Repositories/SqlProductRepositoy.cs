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

        // Get product by ID
        public async Task<Product?> GetProductByIdAsync(int id)
        {
            return await _context.Products
                .Include(p => p.Model)
                .Include(p => p.CaseManufacturer)
                .FirstOrDefaultAsync(p => p.ProductId == id);
        }

        // Update existing product
        public async Task<Product> UpdateProductAsync(Product product)
        {
            var existingProduct = await _context.Products.FindAsync(product.ProductId);
            if (existingProduct == null)
            {
                return null; // Handle case when product is not found
            }

            existingProduct.ModelId = product.ModelId;
            existingProduct.CaseManufacturerId = product.CaseManufacturerId;
            existingProduct.CaseName = product.CaseName;

            _context.Products.Update(existingProduct);
            await _context.SaveChangesAsync();

            return existingProduct;
        }
    }
}
