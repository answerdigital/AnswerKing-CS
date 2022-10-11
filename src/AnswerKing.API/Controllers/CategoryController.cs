using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AnswerKing.Services;
using AnswerKing.Services.DTOs;
using AnswerKing.Services.Interfaces;

namespace AnswerKing.API.Controllers
{
    [Route("api/categories")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly IItemService _itemService;

        public CategoryController(ICategoryService categoryService, IItemService itemService)
        {
            _categoryService = categoryService;
            _itemService = itemService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<CategoryDto>))]
        public async Task<IActionResult> GetAll()
        {
            var categoryDtos = await this._categoryService.GetAll();

            return this.Ok(categoryDtos);
        }

        [HttpGet("{categoryId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CategoryDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int categoryId)
        {
            var categoryDto = await this._categoryService.GetById(categoryId);

            if (categoryDto is null)
            {
                return this.NotFound();
            }

            return this.Ok(categoryDto);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CategoryDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        public async Task<IActionResult> Create([FromBody] CategoryCreateDto createDto)
        {
            if (!ModelState.IsValid)
            {
                return this.BadRequest(ModelState.ValidationState);
            }

            var categoryDto = await this._categoryService.Create(createDto);

            if (categoryDto is null)
            {
                return this.BadRequest($"Category name: {createDto.Name} already exists");
            }

            return this.Ok(categoryDto);
        }

        [HttpPut("{categoryId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CategoryDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int categoryId, [FromBody] CategoryUpdateDto updateDto)
        {
            if (!ModelState.IsValid)
            {
                return this.BadRequest(ModelState.ValidationState);
            }
            
            if (!await this._categoryService.Exists(categoryId))
            {
                return this.NotFound();
            }
            
            var updatedCategoryDto = await this._categoryService.Update(categoryId, updateDto);

            return this.Ok(updatedCategoryDto);
        }

        [HttpDelete("{categoryId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int categoryId)
        {
            var categoryExists = await this._categoryService.Exists(categoryId);
            if (!categoryExists)
            {
                return this.NotFound();
            }

            await this._categoryService.Delete(categoryId);
            
            return this.NoContent();
        }
        
        [HttpPost("{categoryId}/items/{itemId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AddItemToCategory(int categoryId, int itemId)
        {
            var itemExists = await this._itemService.Exists(itemId);
            var categoryExists = await this._categoryService.Exists(categoryId);
            if (!itemExists || !categoryExists)
            {
                return this.NotFound();
            }
            
            if (!await this._itemService.AddCategory(itemId, categoryId))
            {
                return this.BadRequest();
            }

            return this.NoContent();
        }
        
        [HttpDelete("{categoryId}/items/{itemId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RemoveItemFromCategory(int categoryId, int itemId)
        {
            var itemExists = await this._itemService.Exists(itemId);
            var categoryExists = await this._categoryService.Exists(categoryId);
            if (!itemExists || !categoryExists)
            {
                return this.NotFound();
            }

            if (!await this._itemService.RemoveCategory(itemId, categoryId))
            {
                return this.BadRequest();
            }

            return this.NoContent();
        }
        
        [HttpGet("{categoryId}/items")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<ItemDto>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetItemsByCategory(int categoryId)
        {
            var categoryExists = await this._categoryService.Exists(categoryId);
            if (!categoryExists)
            {
                return this.NotFound();
            }
            
            var itemsDtos = await this._itemService.GetByCategory(categoryId);
                
            return this.Ok(itemsDtos);
        }
    }
}
