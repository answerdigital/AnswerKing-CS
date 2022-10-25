using System;
using System.Collections.Generic;
using System.Linq;
using Answer.King.Domain.Repositories.Models;
using Answer.King.Infrastructure.Repositories.Mappings;

namespace Answer.King.Infrastructure.SeedData;

internal static class ProductData
{
    public static IList<Product> Products { get; } = new List<Product>
    {
        ProductFactory.CreateProduct(
            Guid.Parse("8d9142c2-96a0-4808-b00a-c43aee40293f"),
            "Fish",
            "Delicious and satisfying.",
            5.99,
            Category(Guid.Parse("0002015c-1997-4732-a2de-e526323e6146")),
            false),
        ProductFactory.CreateProduct(
            Guid.Parse("89828e46-6cff-438f-be1a-6fa9355cfe24"),
            "Chips",
            "Nothing more to say.",
            2.99,
            Category(Guid.Parse("e32701f8-d644-4d5d-bd52-2a31fdaba3df")),
            false)
    };


    private static Category Category(Guid id)
    {
        return CategoryData.Categories.Where(c => c.Id == id)
            .Select(x => new Category(x.Id, x.Name, x.Description)).SingleOrDefault()!;
    }
}
