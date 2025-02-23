using Microsoft.EntityFrameworkCore;
using Phonecase.Data;
using Phonecase.Models;

namespace Phonecase.Repositories {
    public class SqlProductRepository : IProductRepository {
        private readonly PhoneCaseDbContext _context;

        public SqlProductRepository(PhoneCaseDbContext context) {
            _context = context;
        }

        public async Task<Product> CreateProductAsync(Product product) {
            if (product == null) {
                throw new ArgumentNullException(nameof(product), "Product cannot be null.");
            }

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<IEnumerable<Product>> GetAllAsync() {
            return await _context.Products
                .Include(p => p.Model)
                .Include(p => p.CaseManufacturer)
                .ToListAsync();
        }

        public async Task<Product?> GetProductByIdAsync(int id) {
            if (id <= 0) {
                throw new ArgumentException("Product ID must be greater than zero.", nameof(id));
            }

            return await _context.Products
                .Include(p => p.Model)
                .Include(p => p.CaseManufacturer)
                .FirstOrDefaultAsync(p => p.ProductId == id);
        }

        public async Task<Product?> UpdateProductAsync(Product product) {
            if (product == null) {
                throw new ArgumentNullException(nameof(product), "Product cannot be null.");
            }

            var existingProduct = await _context.Products.FindAsync(product.ProductId);
            if (existingProduct == null) {
                return null; // Product not found
            }

            // Update properties
            existingProduct.ModelId = product.ModelId;
            existingProduct.CaseManufacturerId = product.CaseManufacturerId;
            existingProduct.CaseName = product.CaseName;

            _context.Products.Update(existingProduct);
            await _context.SaveChangesAsync();

            return existingProduct;
        }
    }
}