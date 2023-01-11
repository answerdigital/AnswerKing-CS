﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Answer.King.Domain.Repositories;
using Answer.King.Infrastructure.SeedData;
using Answer.King.Test.Common.CustomTraits;
using NSubstitute;

namespace Answer.King.Infrastructure.UnitTests.SeedData;

[TestCategory(TestType.Unit)]
public class CategoryDataSeederTests
{
    private readonly ILiteDbConnectionFactory dbConnectionFactory = Substitute.For<ILiteDbConnectionFactory>();

    [Fact]
    public void SeedData_DataAlreadySeeded_Returns()
    {
        var categoryDataSeeder = new CategoryDataSeeder();

        var dataSeededFieldInfo =
            typeof(CategoryDataSeeder).GetProperty("DataSeeded", BindingFlags.Instance | BindingFlags.NonPublic);

        dataSeededFieldInfo?.SetValue(categoryDataSeeder, true);

        categoryDataSeeder.SeedData(this.dbConnectionFactory);

        this.dbConnectionFactory.DidNotReceive().GetConnection();
    }
}
