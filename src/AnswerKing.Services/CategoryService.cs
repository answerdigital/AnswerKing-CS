using AnswerKing.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnswerKing.Repositories;
using AnswerKing.Repositories.Interfaces;
using AnswerKing.Services.DTOs;
using AnswerKing.Services.Interfaces;

namespace AnswerKing.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            this._categoryRepository = categoryRepository;
        }
        
        public async Task<List<CategoryDto>> GetAll()
        {
            var categoryEntities = await this._categoryRepository.GetAll();

            var categoryDtos = categoryEntities
                .Select(categoryEntity => new CategoryDto
                {
                    Id = categoryEntity.Id,
                    Name = categoryEntity.Name
                })
                .ToList();

            return categoryDtos;
        }

        public async Task<CategoryDto?> GetById(int categoryId)
        {
            var categoryEntity = await this._categoryRepository.GetById(categoryId);
            if (categoryEntity is null)
            {
                return null;
            }
            
            var categoryDto = new CategoryDto
            {
                Id = categoryEntity.Id,
                Name = categoryEntity.Name
            };

            return categoryDto;
        }

        public async Task<CategoryDto?> Create(CategoryCreateDto createDto)
        {
            var categoryId = await this._categoryRepository.Create(new CategoryEntity
            {
                Name = createDto.Name,
            });

            if (categoryId == null)
            {
                return null;
            }

            var categoryEntity = await this._categoryRepository.GetById(categoryId.GetValueOrDefault());

            return new CategoryDto
            {
                Id = categoryEntity.Id,
                Name = categoryEntity.Name
            };
        }

        public async Task<CategoryDto?> Update(int categoryId, CategoryUpdateDto updateDto)
        {
            var categoryEntity = await this._categoryRepository.GetById(categoryId);

            if (categoryEntity is null)
            {
                return null;
            }

            categoryEntity.Name = updateDto.Name;

            if (!await this._categoryRepository.Update(categoryEntity))
            {
                return null;
            }

            var categoryDto = new CategoryDto
            {
                Id = categoryEntity.Id,
                Name = categoryEntity.Name
            };

            return categoryDto;
        }

        public async Task Delete(int categoryId)
        {
            var categoryEntity = await this._categoryRepository.GetById(categoryId);

            if (categoryEntity is not null)
            {
                await this._categoryRepository.Delete(categoryEntity.Id);
            }
        }

        public async Task<bool> Exists(int categoryId)
        {
            var categoryEntity = await this._categoryRepository.GetById(categoryId);

            return categoryEntity is not null;
        }
    }
}
