﻿using System.Reflection;
using Answer.King.Infrastructure.SeedData;
using Answer.King.Test.Common.CustomTraits;
using NSubstitute;

namespace Answer.King.Infrastructure.UnitTests.SeedData;

[TestCategory(TestType.Unit)]
public class ProductDataSeederTests
{
    private readonly ILiteDbConnectionFactory dbConnectionFactory = Substitute.For<ILiteDbConnectionFactory>();

    [Fact]
    public void SeedData_DataAlreadySeeded_Returns()
    {
        var productDataSeeder = new ProductDataSeeder();

        var dataSeededFieldInfo =
            typeof(ProductDataSeeder).GetProperty("DataSeeded", BindingFlags.Instance | BindingFlags.NonPublic);

        dataSeededFieldInfo?.SetValue(productDataSeeder, true);

        productDataSeeder.SeedData(this.dbConnectionFactory);

        this.dbConnectionFactory.DidNotReceive().GetConnection();
    }
}
