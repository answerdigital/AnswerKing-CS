using System.ComponentModel.DataAnnotations;

namespace AnswerKing.Services.DTOs
{
    public class CategoryUpdateDto
    {
        [Required]
        [StringLength(50)]
        [RegularExpression("^[A-z0-9 ]*$", ErrorMessage = "The field Name must only contain letters, numbers, and spaces.")]
        public string Name { get; set; }
    }
}
