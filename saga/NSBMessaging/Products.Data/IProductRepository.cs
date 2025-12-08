using Products.Domain.Entities;

namespace Products.Data
{
    public interface IProductRepository
    {
        Task<Product> AddProductAsync(Product product);
        Task DeleteProductAsync(int id);
        Task<Product> GetProductAsync(int id);
        Task<IEnumerable<Product>> GetProductsAsync();
        Task<bool> ProductExistsAsync(int id);
        Task<Product> UpdateProductAsync(Product product);
    }
}