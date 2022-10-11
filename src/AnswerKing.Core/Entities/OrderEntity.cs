using System.Collections.Generic;

namespace AnswerKing.Core.Entities
{
    public class OrderEntity : BaseEntity
    {
        public string Status { get; set; }
        public string Address { get; set; }
        public List<OrderItemEntity> Items { get; set; }
    }
}