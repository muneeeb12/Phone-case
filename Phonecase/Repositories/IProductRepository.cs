using Phonecase.Models;

namespace Phonecase.Repositories
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllAsync();
        Task<Product> CreateProductAsync(Product product);

    }
}
