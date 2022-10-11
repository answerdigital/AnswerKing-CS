using AnswerKing.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnswerKing.Services.DTOs;

namespace AnswerKing.Services.Interfaces
{
    public interface IItemService
    {
        Task<List<ItemDto>> GetAll();
        Task<List<ItemDto>> GetByCategory(int categoryId);
        Task<ItemDto?> GetById(int itemId);
        Task<ItemDto?> Create(ItemCreateDto createDto);
        Task<ItemDto?> Update(int itemId, ItemUpdateDto updateDto);
        Task Delete(int itemId);
        Task<bool> Exists(int itemId);
        Task<bool> AddCategory(int itemId, int categoryId);
        Task<bool> RemoveCategory(int itemId, int categoryId);
    }
}
