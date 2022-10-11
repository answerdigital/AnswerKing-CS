using System.ComponentModel.DataAnnotations;

namespace AnswerKing.Services.DTOs
{
    public class CategoryDto
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
    }
}
