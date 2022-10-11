using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AnswerKing.Services.DTOs
{
    public class ItemUpdateDto
    {
        [Required]
        [StringLength(50)]
        [RegularExpression("^[A-z0-9 ]*$", ErrorMessage = "The field Name must only contain letters, numbers, and spaces.")]
        public string Name { get; set; }
        [Required]
        [Range(0, 999999999999.9999)]
        public decimal Price { get; set; }
        [StringLength(500)]
        public string? Description { get; set; }
    }
}
