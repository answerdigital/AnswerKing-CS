using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Answer.King.Domain.Inventory;
using Answer.King.Domain.Inventory.Models;
using Answer.King.Domain.Repositories.Models;
using Answer.King.Infrastructure.Repositories.Mappings;
using Answer.King.Test.Common.CustomTraits;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace Answer.King.Domain.UnitTests.Repositories.Factories;

[TestCategory(TestType.Unit)]
public class CategoryFactoryTests
{
    [Fact]
    public void CreateCategory_ConstructorExists_ReturnsCategory()
    {
        // Arrange / Act
        var result = CategoryFactory.CreateCategory(1, "NAME", "DESC", DateTime.UtcNow, DateTime.UtcNow, new List<ProductId>(), false);

        // Assert
        Assert.IsType<Category>(result);
    }

    [Fact]
    public void CreateCategory_ConstructorNotFound_ReturnsException()
    {
        // Arrange
        var CategoryFactoryConstructorFieldInfo =
        typeof(CategoryFactory).GetField($"<CategoryConstructor>k__BackingField", BindingFlags.Static | BindingFlags.NonPublic);

        var constructor = CategoryFactoryConstructorFieldInfo?.GetValue(null);

        var wrongConstructor = typeof(Product).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)
            .SingleOrDefault(c => c.IsPrivate && c.GetParameters().Length > 0);

        CategoryFactoryConstructorFieldInfo?.SetValue(null, wrongConstructor);

        // Act // Assert
        Assert.Throws<ArgumentException>(() =>
            CategoryFactory.CreateCategory(1, "NAME", "DESC", DateTime.UtcNow, DateTime.UtcNow, new List<ProductId>(), false));

        //Reset static constructor to correct value
        CategoryFactoryConstructorFieldInfo?.SetValue(null, constructor);
    }


}
