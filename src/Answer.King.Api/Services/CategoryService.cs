﻿using Answer.King.Api.RequestModels;
using Answer.King.Domain.Repositories;
using Answer.King.Domain.Inventory;
using Category = Answer.King.Domain.Inventory.Category;
using Answer.King.Domain.Inventory.Models;
using Answer.King.Domain.Repositories.Models;
using Answer.King.Domain.Orders.Models;

namespace Answer.King.Api.Services;

public class CategoryService : ICategoryService
{
    public CategoryService(
        ICategoryRepository categories,
        IProductRepository products)
    {
        this.Categories = categories;
        this.Products = products;
    }

    private ICategoryRepository Categories { get; }

    private IProductRepository Products { get; }


    public async Task<Category?> GetCategory(long categoryId)
    {
        return await this.Categories.Get(categoryId);
    }

    public async Task<IEnumerable<Category>> GetCategories()
    {
        return await this.Categories.Get();
    }

    public async Task<Category> CreateCategory(RequestModels.Category createCategory)
    {
        var categoryProducts = new List<ProductId>();
        var products = new List<Domain.Repositories.Models.Product>();

        foreach (var productId in createCategory.Products)
        {
            var product = await this.Products.Get(productId);

            if (product == null)
            {
                throw new CategoryServiceException("The provided product id is not valid.");
            }

            categoryProducts.Add(new(product.Id));
            products.Add(product);
        }

        var category = new Category(createCategory.Name, createCategory.Description, categoryProducts);

        await this.Categories.Save(category);

        foreach (var product in products)
        {
            product.AddCategory(new CategoryId(category.Id));
            await this.Products.AddOrUpdate(product);
        }

        return category;
    }

    public async Task<Category?> UpdateCategory(long categoryId, RequestModels.Category updateCategory)
    {
        var category = await this.Categories.Get(categoryId);
        if (category == null)
        {
            return null;
        }

        var oldProducts = await this.Products.GetByCategoryId(categoryId);

        if (!oldProducts.Any())
        {
            throw new CategoryServiceException("Could not find any products for this category id.");
        }

        foreach (var oldProduct in oldProducts.ToList())
        {
            // If old product is not still present in updated list, remove link between product and category.
            if (!updateCategory.Products.Contains(oldProduct.Id))
            {
                oldProduct.RemoveCategory(new CategoryId(categoryId));
                await this.Products.AddOrUpdate(oldProduct);

                category.RemoveProduct(new ProductId(oldProduct.Id));
                continue;
            }

            // Else check that the product is still in the database.
            var product = await this.Products.Get(oldProduct.Id);

            if (product == null)
            {
                throw new CategoryServiceException("The provided product id is not valid.");
            }

            product.AddCategory(new CategoryId(categoryId));
            await this.Categories.Save(category);

            category.AddProduct(new ProductId(product.Id));

            updateCategory.Products.Remove(oldProduct.Id);
        }

        // Add any new categories remaining in the list
        foreach (var updateId in updateCategory.Products)
        {
            var product = await this.Products.Get(updateId);

            if (product == null)
            {
                throw new CategoryServiceException("The provided product id is not valid.");
            }

            category.AddProduct(new ProductId(product.Id));
        }

        category.Rename(updateCategory.Name, updateCategory.Description);

        await this.Categories.Save(category);

        return category;
    }

    public async Task<Category?> RetireCategory(long categoryId)
    {
        var category = await this.Categories.Get(categoryId);

        if (category == null)
        {
            return null;
        }

        if (category.Retired)
        {
            throw new CategoryServiceException("The category is already retired.");
        }

        try
        {
            category.RetireCategory();

            await this.Categories.Save(category);

            return category;
        }
        catch (CategoryLifecycleException ex)
        {
            // ignored
            throw new CategoryServiceException(
                $"Cannot retire category whilst there are still products assigned. {string.Join(',', category.Products.Select(p => p.Value))}"
                , ex);
        }
    }
}

[Serializable]
internal class CategoryServiceException : Exception
{
    public CategoryServiceException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public CategoryServiceException() : base()
    {
    }

    public CategoryServiceException(string? message) : base(message)
    {
    }
}
