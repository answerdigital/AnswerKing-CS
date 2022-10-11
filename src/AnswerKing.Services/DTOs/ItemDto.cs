using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AnswerKing.Services.DTOs
{
    public class ItemDto
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public decimal Price { get; set; }
        public string? Description { get; set; }
        [Required]
        public List<CategoryDto> Categories { get; set; }
    }
}
