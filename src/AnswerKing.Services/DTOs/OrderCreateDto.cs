using System.ComponentModel.DataAnnotations;

namespace AnswerKing.Services.DTOs
{
    public class OrderCreateDto
    {
        [Required]
        [StringLength(500)]
        public string Address { get; set; }
    }
}