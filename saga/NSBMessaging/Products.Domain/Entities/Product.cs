using System.ComponentModel.DataAnnotations.Schema;

namespace Products.Domain.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string LongDescription { get; set; }
        [ForeignKey("CategoryId")]
        public int CategoryId { get; set; }
    }
}
