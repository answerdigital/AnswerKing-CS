using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using AnswerKing.API.Controllers;
using AnswerKing.Services;
using AnswerKing.Services.DTOs;
using AnswerKing.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;
using Xunit.Abstractions;

namespace AnswerKing.Tests.Controllers
{
    public class ItemControllerTests
    {
        private readonly ITestOutputHelper _output;
        private readonly IItemService _itemService = Substitute.For<IItemService>();
        private readonly ICategoryService _categoryService = Substitute.For<ICategoryService>();
        private ItemController _sut { get; }

        public ItemControllerTests(ITestOutputHelper output)
        {
            this._output = output;
            this._sut = new ItemController(this._itemService, this._categoryService);
        }

        [Fact]
        public async Task GetAll_ShouldReturnOkObject_WhenItemListIsEmpty()
        {
            // Arrange
            this._itemService.GetAll().Returns(new List<ItemDto>());
            
            // Act
            var result = await this._sut.GetAll() as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
            Assert.IsType<List<ItemDto>>(result.Value);
            Assert.Empty(result.Value as List<ItemDto>);
        }
        
        [Fact]
        public async Task GetAll_ShouldReturnOkObject_WhenItemListIsNotEmpty()
        {
            // Arrange
            this._itemService.GetAll().Returns(new List<ItemDto>
            {
                new ItemDto
                {
                    Id = 0,
                    Name = "Test Item",
                    Price = 0.00M,
                    Description = null,
                    Categories = new List<CategoryDto>(),
                }
            });
            
            // Act
            var result = await this._sut.GetAll() as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
            Assert.IsType<List<ItemDto>>(result.Value);
            Assert.NotEmpty(result.Value as List<ItemDto>);
        }

        [Fact]
        public async Task GetById_ShouldReturnOkObject_WhenItemExists()
        {
            // Arrange
            var testItemId = 0;
            var testItemDto = new ItemDto
            {
                Id = 0,
                Name = "Test",
                Price = 0.00M,
                Description = null,
                Categories = new List<CategoryDto>()
            };
            this._itemService.GetById(testItemId).Returns(testItemDto);
            
            // Act
            var result = await this._sut.GetById(testItemId) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
            Assert.IsType<ItemDto>(result.Value);
            Assert.NotNull(result.Value as ItemDto);
        }

        [Fact]
        public async Task GetById_ShouldReturnNotFound_WhenItemDoesNotExists()
        {
            // Arrange
            var testItemId = 0;
            this._itemService.GetById(testItemId).Returns(null as ItemDto);
            
            // Act
            var result = await this._sut.GetById(testItemId) as NotFoundResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
        }

        [Fact]
        public async Task Create_ShouldReturnOkObject_WhenValidationPasses()
        {
            // Arrange
            var testItemCreateDto = new ItemCreateDto
            {
                Name = "Test",
                Price = 0.00M,
                Description = null,
            };
            var testItemDto = new ItemDto
            {
                Id = 0,
                Name = "Test",
                Price = 0.00M,
                Description = null,
                Categories = new List<CategoryDto>()
            };
            this._itemService.Create(testItemCreateDto).Returns(testItemDto);
            
            // Act
            var result = await this._sut.Create(testItemCreateDto) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
            Assert.IsType<ItemDto>(result.Value);
            Assert.NotNull(result.Value as ItemDto);
        }
        
        [Fact]
        public async Task Create_ShouldReturnBadRequestObject_WhenValidationFails()
        {
            // Arrange
            var testItemCreateDto = new ItemCreateDto
            {
                Name = "Test",
                Price = -5.00M,
                Description = null,
            };
            this._sut.ModelState.AddModelError("Price", "The field Price must be between 0 and 999999999999.9999.");

            // Act
            var result = await this._sut.Create(testItemCreateDto) as BadRequestObjectResult;
            
            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
        }
        
        [Fact]
        public async Task Create_ShouldReturnBadRequestObject_WhenNameIsAlreadyInUse()
        {
            // Arrange
            var testItemCreateDto = new ItemCreateDto
            {
                Name = "Test",
                Price = 0.00M,
                Description = null,
            };
            this._itemService.Create(testItemCreateDto).Returns(null as ItemDto);
            
            // Act
            var result = await this._sut.Create(testItemCreateDto) as BadRequestObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
        }

        [Fact]
        public async Task Update_ShouldReturnOkObject_WhenItemIsSuccessfullyUpdated()
        {
            // Arrange
            var testId = 0;
            var testItemUpdateDto = new ItemUpdateDto
            {
                Name = "Test",
                Price = 0.00M,
                Description = null,
            };
            var testItemDto = new ItemDto
            {
                Id = testId,
                Name = "Test",
                Price = 0.00M,
                Description = null,
                Categories = new List<CategoryDto>()
            };
            this._itemService.Update(testId, testItemUpdateDto).Returns(testItemDto);
            this._itemService.Exists(testId).Returns(true);

            // Act
            var result = await this._sut.Update(testId, testItemUpdateDto) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
            Assert.IsType<ItemDto>(result.Value);
            Assert.NotNull(result.Value as ItemDto);
        }
        
        [Fact]
        public async Task Update_ShouldReturnBadRequestObject_WhenModelIsNotValid()
        {
            // Arrange
            var testId = 0;
            var testItemUpdateDto = new ItemUpdateDto
            {
                Name = null,
                Price = 0.00M,
                Description = null,
            };
            this._itemService.Exists(testId).Returns(true);
            this._sut.ModelState.AddModelError("Name", "The Name field is required.");


            // Act
            var result = await this._sut.Update(testId, testItemUpdateDto) as BadRequestObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
            Assert.IsType<ModelValidationState>(result.Value);
        }
        
        [Fact]
        public async Task Update_ShouldReturnNotFound_WhenItemDoesNotExist()
        {
            // Arrange
            var testId = 0;
            var testItemUpdateDto = new ItemUpdateDto
            {
                Name = "Name",
                Price = 0.00M,
                Description = null,
            };
            this._itemService.Exists(testId).Returns(false);


            // Act
            var result = await this._sut.Update(testId, testItemUpdateDto) as NotFoundResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
        }
        
        [Fact]
        public async Task Delete_ShouldReturnNoContent_WhenItemIsSuccessfullyDeleted()
        {
            // Arrange
            var testId = 0;
            this._itemService.Exists(testId).Returns(true);
            this._itemService.Delete(testId).Returns(Task.FromResult(default(object)));

            // Act
            var result = await this._sut.Delete(testId) as NoContentResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status204NoContent, result.StatusCode);
        }
        
        [Fact]
        public async Task Delete_ShouldReturnNotFound_WhenItemDoesNotExist()
        {
            // Arrange
            var testId = 0;
            this._itemService.Exists(testId).Returns(false);

            // Act
            var result = await this._sut.Delete(testId) as NotFoundResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
        }
        
        [Fact]
        public async Task AddCategory_ShouldReturnNoContent_WhenCategoryIsSuccessfullyAdded()
        {
            // Arrange
            var testItemId = 0;
            var testCategoryId = 0;
            this._itemService.Exists(testItemId).Returns(true);
            this._categoryService.Exists(testCategoryId).Returns(true);
            this._itemService.AddCategory(testItemId, testCategoryId).Returns(Task.FromResult(true));

            // Act
            var result = await this._sut.AddCategoryToItem(testItemId, testCategoryId) as NoContentResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status204NoContent, result.StatusCode);
        }
        
        [Fact]
        public async Task AddCategory_ShouldReturnBadRequest_WhenCategoryIsAlreadyAdded()
        {
            // Arrange
            var testItemId = 0;
            var testCategoryId = 0;
            this._itemService.Exists(testItemId).Returns(true);
            this._categoryService.Exists(testCategoryId).Returns(true);
            this._itemService.AddCategory(testItemId, testCategoryId).Returns(Task.FromResult(false));

            // Act
            var result = await this._sut.AddCategoryToItem(testItemId, testCategoryId) as BadRequestResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
        }
        
        [Fact]
        public async Task AddCategory_ShouldReturnNotFound_WhenItemDoesNotExist()
        {
            // Arrange
            var testItemId = 0;
            var testCategoryId = 0;
            this._itemService.Exists(testItemId).Returns(false);
            this._categoryService.Exists(testCategoryId).Returns(true);
            this._itemService.AddCategory(testItemId, testCategoryId).Returns(Task.FromResult(true));

            // Act
            var result = await this._sut.AddCategoryToItem(testItemId, testCategoryId) as NotFoundResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);        
        }
        
        [Fact]
        public async Task AddCategory_ShouldReturnNotFound_WhenCategoryDoesNotExist()
        {
            // Arrange
            var testItemId = 0;
            var testCategoryId = 0;
            this._itemService.Exists(testItemId).Returns(true);
            this._categoryService.Exists(testCategoryId).Returns(false);
            this._itemService.AddCategory(testItemId, testCategoryId).Returns(Task.FromResult(true));

            // Act
            var result = await this._sut.AddCategoryToItem(testItemId, testCategoryId) as NotFoundResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);        
        }
        
        [Fact]
        public async Task RemoveCategory_ShouldReturnNoContent_WhenCategoryIsSuccessfullyAdded()
        {
            // Arrange
            var testItemId = 0;
            var testCategoryId = 0;
            this._itemService.Exists(testItemId).Returns(true);
            this._categoryService.Exists(testCategoryId).Returns(true);
            this._itemService.RemoveCategory(testItemId, testCategoryId).Returns(Task.FromResult(true));

            // Act
            var result = await this._sut.RemoveCategoryFromItem(testItemId, testCategoryId) as NoContentResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status204NoContent, result.StatusCode);
        }
        
        [Fact]
        public async Task RemoveCategory_ShouldReturnBadRequest_WhenCategoryIsAlreadyAdded()
        {
            // Arrange
            var testItemId = 0;
            var testCategoryId = 0;
            this._itemService.Exists(testItemId).Returns(true);
            this._categoryService.Exists(testCategoryId).Returns(true);
            this._itemService.RemoveCategory(testItemId, testCategoryId).Returns(Task.FromResult(false));

            // Act
            var result = await this._sut.RemoveCategoryFromItem(testItemId, testCategoryId) as BadRequestResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
        }
        
        [Fact]
        public async Task RemoveCategory_ShouldReturnNotFound_WhenItemDoesNotExist()
        {
            // Arrange
            var testItemId = 0;
            var testCategoryId = 0;
            this._itemService.Exists(testItemId).Returns(false);
            this._categoryService.Exists(testCategoryId).Returns(true);
            this._itemService.RemoveCategory(testItemId, testCategoryId).Returns(Task.FromResult(true));

            // Act
            var result = await this._sut.RemoveCategoryFromItem(testItemId, testCategoryId) as NotFoundResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);    
        }
        
        [Fact]
        public async Task RemoveCategory_ShouldReturnNotFound_WhenCategoryDoesNotExist()
        {
            // Arrange
            var testItemId = 0;
            var testCategoryId = 0;
            this._itemService.Exists(testItemId).Returns(true);
            this._categoryService.Exists(testCategoryId).Returns(false);
            this._itemService.RemoveCategory(testItemId, testCategoryId).Returns(Task.FromResult(true));

            // Act
            var result = await this._sut.RemoveCategoryFromItem(testItemId, testCategoryId) as NotFoundResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
        }
    }
}