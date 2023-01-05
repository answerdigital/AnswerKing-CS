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
public class ProductFactoryTests
{
    [Fact]
    public void CreateProduct_ConstructorExists_ReturnsProduct()
    {
        // Arrange / Act
        var result = ProductFactory.CreateProduct(1, "NAME", "DESC", 1, new List<CategoryId>(), new List<TagId>(), false);

        // Assert
        Assert.IsType<Product>(result);
    }

    [Fact]
    public void CreateProduct_ConstructorNotFound_ReturnsException()
    {
        // Arrange
        var ProductFactoryConstructorFieldInfo =
        typeof(ProductFactory).GetField($"<ProductConstructor>k__BackingField", BindingFlags.Static | BindingFlags.NonPublic);

        var constructor = ProductFactoryConstructorFieldInfo?.GetValue(null);

        var wrongConstructor = typeof(Category).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)
            .SingleOrDefault(c => c.IsPrivate && c.GetParameters().Length > 0);

        ProductFactoryConstructorFieldInfo?.SetValue(null, wrongConstructor);

        // Act // Assert
        Assert.Throws<ArgumentException>(() =>
            ProductFactory.CreateProduct(1, "NAME", "DESC", 1, new List<CategoryId>(), new List<TagId>(), false));

        ProductFactoryConstructorFieldInfo?.SetValue(null, constructor);
    }


}
