namespace Contracts.Events
{
    public class OrderCreated : IEvent
    {
        public Guid OrderId { get; init; }
        public Guid CustomerId { get; init; }
        public decimal TotalAmount { get; init; }
        public string Currency { get; init; } = "USD";
        public DateTime CreatedAt { get; init; }
        public string Source { get; init; } = "WebApp";

    }
}
