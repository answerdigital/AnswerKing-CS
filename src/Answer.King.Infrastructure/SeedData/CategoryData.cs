using System;
using System.Collections.Generic;
using Answer.King.Domain.Inventory;
using Answer.King.Domain.Inventory.Models;
using Answer.King.Infrastructure.Repositories.Mappings;

namespace Answer.King.Infrastructure.SeedData;

public static class CategoryData
{
    public static IList<Category> Categories { get; } = new List<Category>
    {
        CategoryFactory.CreateOrder(
            Guid.Parse("0002015c-1997-4732-a2de-e526323e6146"),
            "Seafood",
            "Food from the oceans",
            DateTime.UtcNow.AddDays(-1),
            DateTime.UtcNow.AddHours(-10),
            new List<ProductId>
            {
                new ProductId(Guid.Parse("8d9142c2-96a0-4808-b00a-c43aee40293f"))
            },
            false),
        CategoryFactory.CreateOrder(
            Guid.Parse("e32701f8-d644-4d5d-bd52-2a31fdaba3df"),
            "Sundries",
            "Things that go with things.",
            DateTime.UtcNow.AddDays(-2),
            DateTime.UtcNow.AddHours(-30),
            new List<ProductId>
            {
                new ProductId(Guid.Parse("89828e46-6cff-438f-be1a-6fa9355cfe24"))
            },
            false)
    };
}
