using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AnswerKing.Core.Entities;
using AnswerKing.Repositories.Interfaces;
using AnswerKing.Services;
using AnswerKing.Services.DTOs;
using NSubstitute;
using Xunit;
using Xunit.Abstractions;

namespace AnswerKing.Tests.Services
{
    public class ItemServiceTests
    {
        private readonly ITestOutputHelper _output;
        private readonly IItemRepository _itemRepository = Substitute.For<IItemRepository>();
        private readonly ICategoryRepository _categoryRepository = Substitute.For<ICategoryRepository>();
        private ItemService _sut { get; }

        public ItemServiceTests(ITestOutputHelper output)
        {
            this._output = output;
            this._sut = new ItemService(this._itemRepository, this._categoryRepository);
        }

        [Fact]
        public async Task GetAll_ShouldReturnItemDtoList_WhenListIsEmpty()
        {
            // Arrange
            var testItemEntities = new List<ItemEntity>();
            this._itemRepository.GetAll().Returns(testItemEntities);
            
            // Act
            var result = await this._sut.GetAll();
            
            // Assert
            Assert.NotNull(result);
            Assert.IsType<List<ItemDto>>(result);
            Assert.Empty(result);
        }
        
        [Fact]
        public async Task GetAll_ShouldReturnItemDtoList_WhenListIsNotEmpty()
        {
            // Arrange
            var testItemEntities = new List<ItemEntity>
            {
                new ItemEntity
                {
                    Id = 0,
                    Name = "test",
                    Description = null,
                    Price = 0,
                    Categories = new List<CategoryEntity>()
                }
            };
            this._itemRepository.GetAll().Returns(testItemEntities);
            
            // Act
            var result = await this._sut.GetAll();
            
            // Assert
            Assert.NotNull(result);
            Assert.IsType<List<ItemDto>>(result);
            Assert.NotEmpty(result);
        }
        
        [Fact]
        public async Task GetByCategory_ShouldReturnItemDtoList_WhenListIsEmpty()
        {
            // Arrange
            var testCategoryId = 0;
            var testItemEntities = new List<ItemEntity>();
            this._itemRepository.GetByCategory(testCategoryId).Returns(testItemEntities);
            
            // Act
            var result = await this._sut.GetByCategory(testCategoryId);
            
            // Assert
            Assert.NotNull(result);
            Assert.IsType<List<ItemDto>>(result);
            Assert.Empty(result);
        }
        
        [Fact]
        public async Task GetByCategory_ShouldReturnItemDtoList_WhenListIsNotEmpty()
        {
            // Arrange
            var testCategoryId = 0;
            var testItemEntities = new List<ItemEntity>
            {
                new ItemEntity
                {
                    Id = 0,
                    Name = "test",
                    Description = null,
                    Price = 0,
                    Categories = new List<CategoryEntity>()
                }
            };
            this._itemRepository.GetByCategory(testCategoryId).Returns(testItemEntities);
            
            // Act
            var result = await this._sut.GetByCategory(testCategoryId);
            
            // Assert
            Assert.NotNull(result);
            Assert.IsType<List<ItemDto>>(result);
            Assert.NotEmpty(result);
        }
        
        [Fact]
        public async Task GetById_ShouldReturnItemDto_WhenItemExists()
        {
            // Arrange
            var testItemId = 0;
            var testItemEntity = new ItemEntity
            {
                Id = testItemId,
                Name = "test",
                Description = null,
                Price = 0,
                Categories = new List<CategoryEntity>()
            };
            this._itemRepository.GetById(testItemId).Returns(testItemEntity);
            
            // Act
            var result = await this._sut.GetById(testItemId);
            
            // Assert
            Assert.NotNull(result);
            Assert.IsType<ItemDto>(result);
        }
        
        [Fact]
        public async Task GetById_ShouldReturnNull_WhenItemDoesNotExists()
        {
            // Arrange
            var testItemId = 0;
            this._itemRepository.GetById(testItemId).Returns(null as ItemEntity);

            // Act
            var result = await this._sut.GetById(testItemId);

            // Assert
            Assert.Null(result);
        }
        
        [Fact]
        public async Task Create_ShouldReturnItemDto_WhenItemIsCreated()
        {
            // Arrange
            var testItemId = 0;
            var testItemCreateDto = new ItemCreateDto
            {
                Name = "test",
                Description = null,
                Price = 0
            };
            var testItemEntity = new ItemEntity
            {
                Id = testItemId,
                Name = testItemCreateDto.Name
            };
            this._itemRepository.Create(testItemEntity).ReturnsForAnyArgs(testItemId);
            this._itemRepository.GetById(testItemId).Returns(testItemEntity);

            // Act
            var result = await this._sut.Create(testItemCreateDto);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ItemDto>(result);
            Assert.Equal(testItemId, result.Id);
        }
        
        [Fact]
        public async Task Create_ShouldReturnNull_WhenItemNameAlreadyExists()
        {
            // Arrange
            var testItemId = 0;
            var testItemCreateDto = new ItemCreateDto
            {
                Name = "test",
                Description = null,
                Price = 0
            };
            var testItemEntity = new ItemEntity
            {
                Id = testItemId,
                Name = testItemCreateDto.Name
            };
            this._itemRepository.Create(testItemEntity).ReturnsForAnyArgs((int?) null);

            // Act
            var result = await this._sut.Create(testItemCreateDto);

            // Assert
            Assert.Null(result);
        }
        
        [Fact]
        public async Task Update_ShouldReturnItemDto_WhenItemIsUpdated()
        {
            // Arrange
            var testItemId = 0;
            var testItemUpdateDto = new ItemUpdateDto
            {
                Name = "test",
                Description = null,
                Price = 0,
            };
            var testItemEntity = new ItemEntity
            {
                Id = testItemId,
                Name = testItemUpdateDto.Name,
                Description = testItemUpdateDto.Description,
                Price = testItemUpdateDto.Price,
                Categories = new List<CategoryEntity>()
            };
            var testItemDto = new ItemDto
            {
                Id = testItemId,
                Name = testItemUpdateDto.Name,
                Description = testItemUpdateDto.Description,
                Price = testItemUpdateDto.Price,
                Categories = new List<CategoryDto>()
            };
            this._itemRepository.GetById(testItemId).Returns(testItemEntity);
            this._itemRepository.Update(testItemEntity).Returns(true);
            
            // Act
            var result = await this._sut.Update(testItemId, testItemUpdateDto);
            
            // Assert
            Assert.NotNull(result);
            Assert.IsType<ItemDto>(result);
            Assert.Equal(testItemId, result.Id);
        }
        
        [Fact]
        public async Task Update_ShouldReturnNull_WhenItemDoesNotExist()
        {
            // Arrange
            var testItemId = 0;
            var testItemUpdateDto = new ItemUpdateDto
            {
                Name = "test",
                Description = null,
                Price = 0,
            };
            this._itemRepository.GetById(testItemId).Returns(null as ItemEntity);
            
            // Act
            var result = await this._sut.Update(testItemId, testItemUpdateDto);
            
            // Assert
            Assert.Null(result);
        }
        
        [Fact]
        public async Task Update_ShouldReturnNull_WhenItemNameAlreadyExists()
        {
            // Arrange
            var testItemId = 0;
            var testItemUpdateDto = new ItemUpdateDto
            {
                Name = "test",
                Description = null,
                Price = 0,
            };
            var testItemEntity = new ItemEntity
            {
                Id = testItemId,
                Name = testItemUpdateDto.Name,
                Description = testItemUpdateDto.Description,
                Price = testItemUpdateDto.Price,
                Categories = new List<CategoryEntity>()
            };
            this._itemRepository.GetById(testItemId).Returns(testItemEntity);
            this._itemRepository.Update(testItemEntity).Returns(false);
            
            // Act
            var result = await this._sut.Update(testItemId, testItemUpdateDto);
            
            // Assert
            Assert.Null(result);
        }
        
        [Fact]
        public async Task Delete_ShouldCallItemRepositoryDelete_WhenItemExists()
        {
            // Arrange
            var testItemId = 0;
            var testItemEntity = new ItemEntity()
            {
                Id = testItemId,
                Name = "test",
                Description = null,
                Price = 0,
                Categories = new List<CategoryEntity>()
            };
            this._itemRepository.GetById(testItemId).Returns(testItemEntity);
            this._itemRepository.Delete(testItemId).Returns(true);

            // Act
            await this._sut.Delete(testItemId);

            // Assert
            await this._itemRepository.Received().Delete(testItemId);
        }
        
        [Fact]
        public async Task Delete_ShouldNotCallItemRepositoryDelete_WhenItemDoesNotExist()
        {
            // Arrange
            var testItemId = 0;
            this._itemRepository.GetById(testItemId).Returns(null as ItemEntity);

            // Act
            await this._sut.Delete(testItemId);

            // Assert
            await this._itemRepository.DidNotReceive().Delete(testItemId);
        }
        
        [Fact]
        public async Task Exists_ShouldReturnTrue_WhenItemDoesExist()
        {
            // Arrange
            var testItemId = 0;
            var testItemEntity = new ItemEntity
            {
                Id = testItemId,
                Name = "test",
                Description = null,
                Price = 0,
                Categories = new List<CategoryEntity>()
            };
            this._itemRepository.GetById(testItemId).Returns(testItemEntity);

            // Act
            var result = await this._sut.Exists(testItemId);

            // Assert
            Assert.True(result);
        }
        
        [Fact]
        public async Task Exists_ShouldReturnFalse_WhenItemDoesNotExist()
        {
            // Arrange
            var testItemId = 0;
            this._itemRepository.GetById(testItemId).Returns(null as ItemEntity);

            // Act
            var result = await this._sut.Exists(testItemId);

            // Assert
            Assert.False(result);
        }
        
        [Fact]
        public async Task AddCategory_ShouldReturnTrue_WhenCategoryIsAdded()
        {
            // Arrange
            var testItemId = 0;
            var testCategoryId = 0;
            var testItemEntity = new ItemEntity
            {
                Id = testItemId,
                Name = "test",
                Description = null,
                Price = 0,
                Categories = new List<CategoryEntity>()
            };
            this._itemRepository.GetById(testItemId).Returns(testItemEntity);
            this._itemRepository.AddCategory(testItemId, testCategoryId).Returns(true);

            // Act
            var result = await this._sut.AddCategory(testItemId, testCategoryId);

            // Assert
            Assert.True(result);
        }
        
        [Fact]
        public async Task AddCategory_ShouldReturnFalse_WhenCategoryIsAlreadyAdded()
        {
            // Arrange
            var testItemId = 0;
            var testCategoryId = 0;
            var testCategoryEntity = new CategoryEntity
            {
                Id = testCategoryId,
                Name = "test"
            };
            var testItemEntity = new ItemEntity
            {
                Id = testItemId,
                Name = "test",
                Description = null,
                Price = 0,
                Categories = new List<CategoryEntity> {testCategoryEntity}
            };
            this._itemRepository.GetById(testItemId).Returns(testItemEntity);
            this._itemRepository.AddCategory(testItemId, testCategoryId).Returns(true);

            // Act
            var result = await this._sut.AddCategory(testItemId, testCategoryId);

            // Assert
            Assert.False(result);
        }
        
        [Fact]
        public async Task RemoveCategory_ShouldReturnTrue_WhenCategoryIsRemoved()
        {
            // Arrange
            var testItemId = 0;
            var testCategoryId = 0;
            var testCategoryEntity = new CategoryEntity
            {
                Id = testCategoryId,
                Name = "test"
            };
            var testItemEntity = new ItemEntity
            {
                Id = testItemId,
                Name = "test",
                Description = null,
                Price = 0,
                Categories = new List<CategoryEntity> {testCategoryEntity}
            };
            this._itemRepository.GetById(testItemId).Returns(testItemEntity);
            this._itemRepository.RemoveCategory(testItemId, testCategoryId).Returns(true);

            // Act
            var result = await this._sut.RemoveCategory(testItemId, testCategoryId);

            // Assert
            Assert.True(result);
        }
        
        [Fact]
        public async Task RemoveCategory_ShouldReturnFalse_WhenCategoryIsNotInItem()
        {
            // Arrange
            var testItemId = 0;
            var testCategoryId = 0;
            var testItemEntity = new ItemEntity
            {
                Id = testItemId,
                Name = "test",
                Description = null,
                Price = 0,
                Categories = new List<CategoryEntity>()
            };
            this._itemRepository.GetById(testItemId).Returns(testItemEntity);
            this._itemRepository.RemoveCategory(testItemId, testCategoryId).Returns(true);

            // Act
            var result = await this._sut.RemoveCategory(testItemId, testCategoryId);

            // Assert
            Assert.False(result);
        }
    }
}