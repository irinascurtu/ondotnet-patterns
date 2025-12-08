namespace Ordering.Domain.Entities
{
    public class Customer
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public required string Email { get; set; }
        public string Phone { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]

        public ICollection<Order> Orders { get; set; }
    }
}
