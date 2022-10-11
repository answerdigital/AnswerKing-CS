using AnswerKing.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnswerKing.Repositories.Interfaces;
using AnswerKing.Services.DTOs;
using AnswerKing.Services.Interfaces;

namespace AnswerKing.Services
{
    public class ItemService : IItemService
    {
        private readonly IItemRepository _itemRepository;
        private readonly ICategoryRepository _categoryRepository;

        public ItemService(IItemRepository itemRepository, ICategoryRepository categoryRepository)
        {
            this._itemRepository = itemRepository;
            this._categoryRepository = categoryRepository;
        }
        
        public async Task<List<ItemDto>> GetAll()
        {
            var itemEntities = await this._itemRepository.GetAll();
            
            var itemDtos = itemEntities
                .Select(itemEntity => new ItemDto
                {
                    Id = itemEntity.Id,
                    Name = itemEntity.Name,
                    Price = itemEntity.Price,
                    Description = itemEntity.Description,
                    Categories = itemEntity.Categories
                        .Select(categoryEntity => new CategoryDto()
                        {
                            Id = categoryEntity.Id,
                            Name = categoryEntity.Name
                        })
                        .ToList()
                })
                .ToList();

            return itemDtos;
        }
        
        public async Task<List<ItemDto>> GetByCategory(int categoryId)
        {
            var itemEntities = await this._itemRepository.GetByCategory(categoryId);
            
            var itemDtos = itemEntities
                .Select(itemEntity => new ItemDto
                {
                    Id = itemEntity.Id,
                    Name = itemEntity.Name,
                    Price = itemEntity.Price,
                    Description = itemEntity.Description,
                    Categories = itemEntity.Categories
                        .Select(categoryEntity => new CategoryDto()
                        {
                            Id = categoryEntity.Id,
                            Name = categoryEntity.Name
                        })
                        .ToList()
                })
                .ToList();

            return itemDtos;
        }

        public async Task<ItemDto?> GetById(int itemId)
        {
            var itemEntity = await this._itemRepository.GetById(itemId);
            if (itemEntity is null)
            {
                return null;
            }

            var itemDto = new ItemDto
            {
                Id = itemEntity.Id,
                Name = itemEntity.Name,
                Price = itemEntity.Price,
                Description = itemEntity.Description,
                Categories = itemEntity.Categories
                    .Select(categoryEntity => new CategoryDto()
                    {
                        Id = categoryEntity.Id,
                        Name = categoryEntity.Name
                    })
                    .ToList()
            };

            return itemDto;
        }

        public async Task<ItemDto?> Create(ItemCreateDto createDto)
        {
            var itemId = await this._itemRepository.Create(new ItemEntity
            {
                Name = createDto.Name,
                Price = createDto.Price,
                Description = createDto.Description,
                Categories = new List<CategoryEntity>()
            });

            if (itemId is null)
            {
                return null;
            }

            var itemEntity = await this._itemRepository.GetById(itemId.GetValueOrDefault());
            
            return new ItemDto
            {
                Id = itemEntity.Id,
                Name = itemEntity.Name,
                Price = itemEntity.Price,
                Description = itemEntity.Description,
                Categories = new List<CategoryDto>()
            };
        }

        public async Task<ItemDto?> Update(int itemId, ItemUpdateDto updateDto)
        {
            var itemEntity = await this._itemRepository.GetById(itemId);
            
            if (itemEntity is null)
            {
                return null;
            }

            itemEntity.Id = itemId;
            itemEntity.Name = updateDto.Name;
            itemEntity.Price = updateDto.Price;
            itemEntity.Description = updateDto.Description;

            if (!await this._itemRepository.Update(itemEntity))
            {
                return null;
            }

            var itemDto = new ItemDto
            {
                Id = itemEntity.Id,
                Name = itemEntity.Name,
                Price = itemEntity.Price,
                Description = itemEntity.Description,
                Categories = itemEntity.Categories
                    .Select(categoryEntity => new CategoryDto
                    {
                        Id = categoryEntity.Id,
                        Name = categoryEntity.Name
                    })
                    .ToList()
            };

            return itemDto;
        }

        public async Task Delete(int itemId)
        {
            var itemEntity = await this._itemRepository.GetById(itemId);

            if (itemEntity is not null)
            {
                await this._itemRepository.Delete(itemEntity.Id);
            }
        }
        
        public async Task<bool> Exists(int itemId)
        {
            var itemEntity = await this._itemRepository.GetById(itemId);

            return itemEntity is not null;
        }

        public async Task<bool> AddCategory(int itemId, int categoryId)
        {
            var itemEntity = await this._itemRepository.GetById(itemId);
            
            if (itemEntity?.Categories.FirstOrDefault(category => category.Id == categoryId) is not null)
            {
                return false;
            }
            
            return await this._itemRepository.AddCategory(itemId, categoryId);
        }

        public async Task<bool> RemoveCategory(int itemId, int categoryId)
        {
            var itemEntity = await this._itemRepository.GetById(itemId);
            
            if (itemEntity?.Categories.FirstOrDefault(category => category.Id == categoryId) is null)
            {
                return false;
            }
            
            return await this._itemRepository.RemoveCategory(itemId, categoryId);
        }
    }
}
