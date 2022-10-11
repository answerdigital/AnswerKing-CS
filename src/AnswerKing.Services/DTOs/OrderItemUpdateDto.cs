using System.ComponentModel.DataAnnotations;

namespace AnswerKing.Services.DTOs
{
    public class OrderItemUpdateDto
    {
        [Required]
        public int Quantity { get; set; }
    }
}