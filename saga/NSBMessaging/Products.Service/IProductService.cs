using Products.Domain.Entities;

namespace ProductsApi.Service
{
    public interface IProductService
    {
        Task<Product> AddProductAsync(Product product);
        Task DeleteProductAsync(int id);
        Task<Product> GetProductAsync(int id);
        Task<IEnumerable<Product>> GetProductsAsync();
        Task<Product> UpdateProductAsync(Product product);
        Task<bool> ProductExistsAsync(int id);
    }
}