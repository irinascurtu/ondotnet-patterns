using Ordering.Domain.Entities;

namespace Ordering.Service
{
    public interface IOrderService
    {
        Task<Order> AddOrderAsync(Order order);
        Task DeleteOrderAsync(Guid id);
        Task<Order> GetOrderAsync(Guid id);
        Task<IEnumerable<Order>> GetOrdersAsync();
        Task<int> UpdateOrderAsync(Order order);
        Task<bool> OrderExistsAsync(Guid id);
        Task<int> SaveChangesAsync();
    }
}