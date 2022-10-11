using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AnswerKing.API.Controllers;
using AnswerKing.Services;
using AnswerKing.Services.DTOs;
using AnswerKing.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using NSubstitute;
using Xunit;
using Xunit.Abstractions;

namespace AnswerKing.Tests.Controllers
{
    public class CategoryControllerTests
    {
        private readonly ITestOutputHelper _output;
        private readonly ICategoryService _categoryService = Substitute.For<ICategoryService>();
        private readonly IItemService _itemService = Substitute.For<IItemService>();
        private CategoryController _sut { get; }
        
        public CategoryControllerTests(ITestOutputHelper output)
        {
            this._output = output;
            this._sut = new CategoryController(this._categoryService, this._itemService);
        }

        [Fact]
        public async Task GetAll_ShouldReturnOkObject_WhenListIsEmpty()
        {
            // Arrange
            this._categoryService.GetAll().Returns(new List<CategoryDto>());
            
            // Act
            var result = await this._sut.GetAll() as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
            Assert.IsType<List<CategoryDto>>(result.Value);
            Assert.Empty(result.Value as List<CategoryDto>);
        }
        
        [Fact]
        public async Task GetAll_ShouldReturnOkObject_WhenListIsNotEmpty()
        {
            // Arrange
            this._categoryService.GetAll().Returns(new List<CategoryDto>
            {
                new CategoryDto
                {
                    Id = 0,
                    Name = "Test Item",
                }
            });
            
            // Act
            var result = await this._sut.GetAll() as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
            Assert.IsType<List<CategoryDto>>(result.Value);
            Assert.NotEmpty(result.Value as List<CategoryDto>);
        }
        
        [Fact]
        public async Task GetById_ShouldReturnOkObject_WhenCategoryExists()
        {
            // Arrange
            var testCategoryId = 0;
            var testCategoryDto = new CategoryDto
            {
                Id = 0,
                Name = "Test",
            };
            this._categoryService.GetById(testCategoryId).Returns(testCategoryDto);
            
            // Act
            var result = await this._sut.GetById(testCategoryId) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
            Assert.IsType<CategoryDto>(result.Value);
            Assert.NotNull(result.Value as CategoryDto);
        }
        
        [Fact]
        public async Task GetById_ShouldReturnNotFound_WhenCategoryDoesNotExist()
        {
            // Arrange
            var testCategoryId = 0;
            this._categoryService.GetById(testCategoryId).Returns(null as CategoryDto);
            
            // Act
            var result = await this._sut.GetById(testCategoryId) as NotFoundResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
        }
        
        [Fact]
        public async Task Create_ShouldReturnOkObject_WhenValidationPasses()
        {
            // Arrange
            var testCategoryCreateDto = new CategoryCreateDto
            {
                Name = "Test",
            };
            var testCategoryDto = new CategoryDto
            {
                Id = 0,
                Name = "Test",
            };
            this._categoryService.Create(testCategoryCreateDto).Returns(testCategoryDto);
            
            // Act
            var result = await this._sut.Create(testCategoryCreateDto) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
            Assert.IsType<CategoryDto>(result.Value);
            Assert.NotNull(result.Value as CategoryDto);
        }
        
        [Fact]
        public async Task Create_ShouldReturnBadRequestObject_WhenValidationFails()
        {
            // Arrange
            var testCategoryCreateDto = new CategoryCreateDto
            {
                Name = null,
            };
            this._sut.ModelState.AddModelError("Name", "The name field is required.");

            // Act
            var result = await this._sut.Create(testCategoryCreateDto) as BadRequestObjectResult;
            
            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
        }
        
        [Fact]
        public async Task Create_ShouldReturnBadRequestObject_WhenNameIsAlreadyInUse()
        {
            // Arrange
            var testCategoryCreateDto = new CategoryCreateDto
            {
                Name = "Test",
            };
            this._categoryService.Create(testCategoryCreateDto).Returns(null as CategoryDto);
            
            // Act
            var result = await this._sut.Create(testCategoryCreateDto) as BadRequestObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
        }
        
        [Fact]
        public async Task Update_ShouldReturnOkObject_WhenCategoryIsSuccessfullyUpdated()
        {
            // Arrange
            var testId = 0;
            var testCategoryUpdateDto = new CategoryUpdateDto
            {
                Name = "Test",
            };
            var testCategoryDto = new CategoryDto
            {
                Id = testId,
                Name = "Test",
            };
            this._categoryService.Update(testId, testCategoryUpdateDto).Returns(testCategoryDto);
            this._categoryService.Exists(testId).Returns(true);

            // Act
            var result = await this._sut.Update(testId, testCategoryUpdateDto) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
            Assert.IsType<CategoryDto>(result.Value);
            Assert.NotNull(result.Value as CategoryDto);
        }
        
        [Fact]
        public async Task Update_ShouldReturnBadRequestObject_WhenModelIsNotValid()
        {
            // Arrange
            var testId = 0;
            var testCategoryUpdateDto = new CategoryUpdateDto
            {
                Name = null,
            };
            this._categoryService.Exists(testId).Returns(true);
            this._sut.ModelState.AddModelError("Name", "The Name field is required.");


            // Act
            var result = await this._sut.Update(testId,  testCategoryUpdateDto) as BadRequestObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
            Assert.IsType<ModelValidationState>(result.Value);
        }
        
        [Fact]
        public async Task Update_ShouldReturnNotFound_WhenCategoryDoesNotExist()
        {
            // Arrange
            var testId = 0;
            var testCategoryUpdateDto = new CategoryUpdateDto
            {
                Name = null,
            };
            this._categoryService.Exists(testId).Returns(false);


            // Act
            var result = await this._sut.Update(testId, testCategoryUpdateDto) as NotFoundResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
        }
        
        [Fact]
        public async Task Delete_ShouldReturnNoContent_WhenItemIsSuccessfullyDeleted()
        {
            // Arrange
            var testId = 0;
            this._categoryService.Exists(testId).Returns(true);
            this._categoryService.Delete(testId).Returns(Task.FromResult(default(object)));

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
            this._categoryService.Exists(testId).Returns(false);

            // Act
            var result = await this._sut.Delete(testId) as NotFoundResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
        }
        
        [Fact]
        public async Task AddItem_ShouldReturnNoContent_WhenItemIsSuccessfullyAdded()
        {
            // Arrange
            var testItemId = 0;
            var testCategoryId = 0;
            this._itemService.Exists(testItemId).Returns(true);
            this._categoryService.Exists(testCategoryId).Returns(true);
            this._itemService.AddCategory(testItemId, testCategoryId).Returns(Task.FromResult(true));

            // Act
            var result = await this._sut.AddItemToCategory(testCategoryId, testItemId) as NoContentResult;

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
            var result = await this._sut.AddItemToCategory(testCategoryId, testItemId) as BadRequestResult;

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
            var result = await this._sut.AddItemToCategory(testCategoryId, testItemId) as NotFoundResult;

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
            var result = await this._sut.AddItemToCategory(testCategoryId, testItemId) as NotFoundResult;

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
            var result = await this._sut.RemoveItemFromCategory(testCategoryId, testItemId) as NoContentResult;

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
            var result = await this._sut.RemoveItemFromCategory(testCategoryId, testItemId) as BadRequestResult;

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
            var result = await this._sut.RemoveItemFromCategory(testCategoryId, testItemId) as NotFoundResult;

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
            var result = await this._sut.RemoveItemFromCategory(testCategoryId, testItemId) as NotFoundResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
        }

        [Fact]
        public async Task GetItemsByCategory_ShouldReturnOkObject_WhenCategoryDoesExist()
        {
            // Arrange
            var testCategoryId = 0;
            var testItemDtoList = new List<ItemDto>();
            this._categoryService.Exists(testCategoryId).Returns(true);
            this._itemService.GetByCategory(testCategoryId).Returns(testItemDtoList);

            // Act
            var result = await this._sut.GetItemsByCategory(testCategoryId) as OkObjectResult;
            
            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
            Assert.IsType<List<ItemDto>>(result.Value);
        }
        
        [Fact]
        public async Task GetItemsByCategory_ShouldReturnNotFound_WhenCategoryDoesNotExist()
        {
            // Arrange
            var testCategoryId = 0;
            this._categoryService.Exists(testCategoryId).Returns(false);

            // Act
            var result = await this._sut.GetItemsByCategory(testCategoryId) as NotFoundResult;
            
            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
        }
    }
}