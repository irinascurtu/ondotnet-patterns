using Microsoft.EntityFrameworkCore;
using Ordering.Domain.Entities;

namespace Ordering.Domain
{
    public class OrderContext : DbContext
    {
        public OrderContext(DbContextOptions<OrderContext> options) : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>(e =>
            {
                e.HasKey(x => x.Id);

                e.Property(x => x.Id)
                    .ValueGeneratedNever();
                
                e.Property(x => x.OrderDate)
                .HasDefaultValueSql("SYSUTCDATETIME()");

            });
            base.OnModelCreating(modelBuilder);

        }
    }
}
