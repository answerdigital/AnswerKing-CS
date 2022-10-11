using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Threading.Tasks;
using AnswerKing.Services;
using AnswerKing.Services.DTOs;
using AnswerKing.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace AnswerKing.API.Controllers
{
    [Route("api/items")]
    [ApiController]
    public class ItemController : ControllerBase
    {
        private readonly IItemService _itemService;
        private readonly ICategoryService _categoryService;

        public ItemController(IItemService itemService, ICategoryService categoryService)
        {
            this._itemService = itemService;
            this._categoryService = categoryService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<ItemDto>))]
        public async Task<IActionResult> GetAll()
        {
            var itemsDtos = await this._itemService.GetAll();
            
            return this.Ok(itemsDtos);
        }

        [HttpGet("{itemId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ItemDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int itemId)
        {
            var itemDto = await this._itemService.GetById(itemId);

            if (itemDto is null)
            {
                return this.NotFound();
            }

            return this.Ok(itemDto);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ItemDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> Create([FromBody] ItemCreateDto createDto)
        {
            if (!ModelState.IsValid)
            {
                return this.BadRequest(ModelState.ValidationState);
            }

            var itemDto = await this._itemService.Create(createDto);

            if (itemDto is null)
            {
                var problemDetails = new ProblemDetails();
                problemDetails.Title = $"Item name \"{createDto.Name}\" already exists.";
                problemDetails.Status = StatusCodes.Status400BadRequest;
                return this.BadRequest(problemDetails);
            }

            return this.Ok(itemDto);
        }

        [HttpPut("{itemId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ItemDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int itemId, [FromBody] ItemUpdateDto updateDto)
        {
            if (!ModelState.IsValid)
            {
                return this.BadRequest(ModelState.ValidationState);
            }

            var itemExists = await this._itemService.Exists(itemId);
            if (!itemExists)
            {
                return this.NotFound();
            }

            var updatedItemDto = await this._itemService.Update(itemId, updateDto);

            return this.Ok(updatedItemDto);
        }

        [HttpDelete("{itemId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete (int itemId)
        {
            var itemExists = await this._itemService.Exists(itemId);
            if (!itemExists)
            {
                return this.NotFound();
            }

            await this._itemService.Delete(itemId);
            
            return this.NoContent();
        }

        [HttpPost("{itemId}/categories/{categoryId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AddCategoryToItem(int itemId, int categoryId)
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
        
        [HttpDelete("{itemId}/categories/{categoryId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RemoveCategoryFromItem(int itemId, int categoryId)
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
    }
}
