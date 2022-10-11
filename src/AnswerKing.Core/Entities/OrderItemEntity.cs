using System.Collections.Generic;

namespace AnswerKing.Core.Entities
{
    public class OrderItemEntity : BaseEntity
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string? Description { get; set; }
        public int Quantity { get; set; }
    }
}