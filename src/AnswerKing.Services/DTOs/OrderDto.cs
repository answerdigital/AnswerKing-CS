using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AnswerKing.Services.DTOs
{
    public class OrderDto
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Status { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public decimal Total { get; set; }
        [Required]
        public List<OrderItemDto> Items { get; set; }
    }
}