using Ordering.Domain.Entities;

namespace Contracts.Models
{

    public class OrderModel
    {

        public OrderModel()
        {
            Status = OrderStatus.Received;
            Id = Guid.NewGuid();

        }

        public OrderStatus Status { get; set; }

        public string DeliveryInstructions { get; set; }
        public string CustomerName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }

        public Guid Id { get; init; }

        public List<OrderItemModel> OrderItems { get; set; }
    }
}
