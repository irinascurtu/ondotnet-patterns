using Microsoft.EntityFrameworkCore;
using Ordering.Domain;
using Ordering.Domain.Entities;

namespace Ordering.Data
{
    public class OrderRepository : IOrderRepository
    {
        private readonly OrderContext _context;

        public OrderRepository(OrderContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Order>> GetOrdersAsync()
        {
            return await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderItems)
                .ToListAsync();
        }


        public async Task<Order> GetOrderAsync(Guid id)
        {
            return await _context.Orders.FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<Order> AddOrderAsync(Order order)
        {
            _context.Orders.Add(order);
           // await _context.SaveChangesAsync();
            return order;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }


        public async Task<int> UpdateOrderAsync(Order order)
        {
            _context.Entry(order).State = EntityState.Modified;
            return await _context.SaveChangesAsync();
        }

        public async Task DeleteOrderAsync(Guid id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> OrderExistsAsync(Guid id)
        {
            return await _context.Orders.AnyAsync(e => e.Id == id);
        }

    }
}
