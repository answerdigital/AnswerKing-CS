using AnswerKing.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnswerKing.Services.DTOs;

namespace AnswerKing.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<List<CategoryDto>> GetAll();
        Task<CategoryDto?> GetById(int categoryId);
        Task<CategoryDto?> Create(CategoryCreateDto createDto);
        Task<CategoryDto?> Update(int categoryId, CategoryUpdateDto updateDto);
        Task Delete(int categoryId);
        Task<bool> Exists(int categoryId);
    }
}
