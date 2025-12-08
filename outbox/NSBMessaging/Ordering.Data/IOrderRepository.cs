using Ordering.Domain.Entities;

namespace Ordering.Data
{
    public interface IOrderRepository
    {
        Task<Order> AddOrderAsync(Order order);
        Task DeleteOrderAsync(Guid id);
        Task<Order> GetOrderAsync(Guid id);
        Task<IEnumerable<Order>> GetOrdersAsync();
        Task<bool> OrderExistsAsync(Guid id);
      
        Task<int> SaveChangesAsync();
        Task<int> UpdateOrderAsync(Order order);
    }
}