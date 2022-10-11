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
    public class CategoryServiceTests
    {
        private readonly ITestOutputHelper _output;
        private readonly ICategoryRepository _categoryRepository = Substitute.For<ICategoryRepository>();
        private CategoryService _sut { get; }

        public CategoryServiceTests(ITestOutputHelper output)
        {
            this._output = output;
            this._sut = new CategoryService(this._categoryRepository);
        }

        [Fact]
        public async Task GetAll_ShouldReturnCategoryDtoList_WhenListIsEmpty()
        {
            // Arrange
            var testCategoryEntities = new List<CategoryEntity>();
            this._categoryRepository.GetAll().Returns(testCategoryEntities);

            // Act
            var result = await this._sut.GetAll();

            // Assert
            Assert.NotNull(result);
            Assert.IsType<List<CategoryDto>>(result);
            Assert.Empty(result);
        }
        
        [Fact]
        public async Task GetAll_ShouldReturnCategoryDtoList_WhenListIsNotEmpty()
        {
            // Arrange
            var testCategoryEntities = new List<CategoryEntity>
            {
                new CategoryEntity
                {
                    Id = 0,
                    Name = "test"
                }
            };
            this._categoryRepository.GetAll().Returns(testCategoryEntities);

            // Act
            var result = await this._sut.GetAll();

            // Assert
            Assert.NotNull(result);
            Assert.IsType<List<CategoryDto>>(result);
            Assert.NotEmpty(result);
        }
        
        [Fact]
        public async Task GetById_ShouldReturnCategoryDto_WhenCategoryExists()
        {
            // Arrange
            var testCategoryId = 0;
            var testCategoryEntity = new CategoryEntity
            {
                Id = testCategoryId,
                Name = "test"
            };
            this._categoryRepository.GetById(testCategoryId).Returns(testCategoryEntity);

            // Act
            var result = await this._sut.GetById(testCategoryId);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<CategoryDto>(result);
        }
        
        [Fact]
        public async Task GetById_ShouldReturnNull_WhenCategoryDoesNotExist()
        {
            // Arrange
            var testCategoryId = 0;
            this._categoryRepository.GetById(testCategoryId).Returns(null as CategoryEntity);

            // Act
            var result = await this._sut.GetById(testCategoryId);

            // Assert
            Assert.Null(result);
        }
        
        [Fact]
        public async Task Create_ShouldReturnCategoryDto_WhenCategoryIsCreated()
        {
            // Arrange
            var testCategoryId = 0;
            var testCategoryCreateDto = new CategoryCreateDto
            {
                Name = "test"
            };
            var testCategoryEntity = new CategoryEntity
            {
                Id = testCategoryId,
                Name = testCategoryCreateDto.Name
            };
            this._categoryRepository.Create(testCategoryEntity).ReturnsForAnyArgs(testCategoryId);
            this._categoryRepository.GetById(testCategoryId).Returns(testCategoryEntity);

            // Act
            var result = await this._sut.Create(testCategoryCreateDto);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<CategoryDto>(result);
            Assert.Equal(testCategoryId, result.Id);
        }
        
        [Fact]
        public async Task Create_ShouldReturnNull_WhenCategoryNameAlreadyExists()
        {
            // Arrange
            var testCategoryId = 0;
            var testCategoryCreateDto = new CategoryCreateDto
            {
                Name = "test"
            };
            var testCategoryEntity = new CategoryEntity
            {
                Id = testCategoryId,
                Name = testCategoryCreateDto.Name
            };
            this._categoryRepository.Create(testCategoryEntity).ReturnsForAnyArgs((int?) null);

            // Act
            var result = await this._sut.Create(testCategoryCreateDto);

            // Assert
            Assert.Null(result);
        }
        
        [Fact]
        public async Task Update_ShouldReturnCategoryDto_WhenCategoryIsUpdated()
        {
            // Arrange
            var testCategoryId = 0;
            var testCategoryUpdateDto = new CategoryUpdateDto
            {
                Name = "test"
            };
            var testCategoryEntity = new CategoryEntity
            {
                Id = testCategoryId,
                Name = testCategoryUpdateDto.Name
            };
            var testCategoryDto = new CategoryDto
            {
                Id = testCategoryId,
                Name = testCategoryUpdateDto.Name
            };
            this._categoryRepository.GetById(testCategoryId).Returns(testCategoryEntity);
            this._categoryRepository.Update(testCategoryEntity).Returns(true);
            
            // Act
            var result = await this._sut.Update(testCategoryId, testCategoryUpdateDto);
            
            // Assert
            Assert.NotNull(result);
            Assert.IsType<CategoryDto>(result);
            Assert.Equal(testCategoryId, result.Id);
        }
        
        [Fact]
        public async Task Update_ShouldReturnNull_WhenCategoryDoesNotExist()
        {
            // Arrange
            var testCategoryId = 0;
            var testCategoryUpdateDto = new CategoryUpdateDto
            {
                Name = "test"
            };
            this._categoryRepository.GetById(testCategoryId).Returns(null as CategoryEntity);
            
            // Act
            var result = await this._sut.Update(testCategoryId, testCategoryUpdateDto);
            
            // Assert
            Assert.Null(result);
        }
        
        [Fact]
        public async Task Update_ShouldReturnNull_WhenCategoryNameAlreadyExists()
        {
            // Arrange
            var testCategoryId = 0;
            var testCategoryUpdateDto = new CategoryUpdateDto
            {
                Name = "test"
            };
            var testCategoryEntity = new CategoryEntity
            {
                Id = testCategoryId,
                Name = testCategoryUpdateDto.Name
            };
            this._categoryRepository.GetById(testCategoryId).Returns(testCategoryEntity);
            this._categoryRepository.Update(testCategoryEntity).Returns(false);
            
            // Act
            var result = await this._sut.Update(testCategoryId, testCategoryUpdateDto);
            
            // Assert
            Assert.Null(result);
        }
        
        [Fact]
        public async Task Delete_ShouldCallCategoryRepositoryDelete_WhenCategoryExists()
        {
            // Arrange
            var testCategoryId = 0;
            var testCategoryEntity = new CategoryEntity
            {
                Id = testCategoryId,
                Name = "test"
            };
            this._categoryRepository.GetById(testCategoryId).Returns(testCategoryEntity);
            this._categoryRepository.Delete(testCategoryId).Returns(true);

            // Act
            await this._sut.Delete(testCategoryId);

            // Assert
            await this._categoryRepository.Received().Delete(testCategoryId);
        }
        
        [Fact]
        public async Task Delete_ShouldNotCallCategoryRepositoryDelete_WhenCategoryDoesNotExist()
        {
            // Arrange
            var testCategoryId = 0;
            this._categoryRepository.GetById(testCategoryId).Returns(null as CategoryEntity);

            // Act
            await this._sut.Delete(testCategoryId);

            // Assert
            await this._categoryRepository.DidNotReceive().Delete(testCategoryId);
        }
        
        [Fact]
        public async Task Exists_ShouldReturnTrue_WhenCategoryDoesExist()
        {
            // Arrange
            var testCategoryId = 0;
            var testCategoryEntity = new CategoryEntity
            {
                Id = testCategoryId,
                Name = "test"
            };
            this._categoryRepository.GetById(testCategoryId).Returns(testCategoryEntity);

            // Act
            var result = await this._sut.Exists(testCategoryId);

            // Assert
            Assert.True(result);
        }
        
        [Fact]
        public async Task Exists_ShouldReturnFalse_WhenCategoryDoesNotExists()
        {
            // Arrange
            var testCategoryId = 0;
            this._categoryRepository.GetById(testCategoryId).Returns(null as CategoryEntity);

            // Act
            var result = await this._sut.Exists(testCategoryId);

            // Assert
            Assert.False(result);
        }
    }
}