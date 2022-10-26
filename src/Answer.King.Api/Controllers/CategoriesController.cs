using Answer.King.Api.Services;
using Microsoft.AspNetCore.Mvc;
using CategoryDto = Answer.King.Api.RequestModels.CategoryDto;
using Product = Answer.King.Domain.Orders.Models.Product;

namespace Answer.King.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
public class CategoriesController : ControllerBase
{
    public CategoriesController(ICategoryService categories, IProductService products)
    {
        this.Categories = categories;
        this.Products = products;
    }

    private ICategoryService Categories { get; }

    private IProductService Products { get; }

    /// <summary>
    /// Get all categories.
    /// </summary>
    /// <returns></returns>
    /// <response code="200">When all the categories have been returned.</response>
    // GET api/categories
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Domain.Inventory.Category>), 200)]
    public async Task<IActionResult> GetAll()
    {
        return this.Ok(await this.Categories.GetCategories());
    }

    /// <summary>
    /// Get a single category.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <response code="200">When the category with the provided <paramref name="id"/> has been found.</response>
    /// <response code="404">When the category with the given <paramref name="id"/> does not exist</response>
    // GET api/categories/{GUID}
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Domain.Inventory.Category), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetOne(Guid id)
    {
        var category = await this.Categories.GetCategory(id);
        if (category == null)
        {
            return this.NotFound();
        }

        return this.Ok(category);
    }

    /// <summary>
    /// Create a new category.
    /// </summary>
    /// <param name="createCategory"></param>
    /// <response code="201">When the category has been created.</response>
    /// <response code="400">When invalid parameters are provided.</response>
    // POST api/categories
    [HttpPost]
    [ProducesResponseType(typeof(Domain.Inventory.Category), 201)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    public async Task<IActionResult> Post([FromBody] CategoryDto createCategory)
    {
        var category = await this.Categories.CreateCategory(createCategory);

        return this.CreatedAtAction(nameof(this.GetOne), new { category.Id }, category);
    }

    /// <summary>
    /// Update an existing category.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="updateCategory"></param>
    /// <response code="200">When the category has been updated.</response>
    /// <response code="400">When invalid parameters are provided.</response>
    /// <response code="404">When the category with the given <paramref name="id"/> does not exist.</response>
    // PUT api/categories/{GUID}
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(Domain.Inventory.Category), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Put(Guid id, [FromBody] CategoryDto updateCategory)
    {
        var category = await this.Categories.UpdateCategory(id, updateCategory);
        if (category == null)
        {
            return this.NotFound();
        }

        return this.Ok(category);
    }

    /// <summary>
    /// Retire an existing category.
    /// </summary>
    /// <param name="id"></param>
    /// <response code="200">When the category has been retired.</response>
    /// <response code="400">When invalid parameters are provided.</response>
    /// <response code="404">When the category with the given <paramref name="id"/> does not exist.</response>
    // DELETE api/categories/{GUID}
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(Domain.Inventory.Category), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Retire(Guid id)
    {
        try
        {
            var category = await this.Categories.RetireCategory(id);

            if (category == null)
            {
                return this.NotFound();
            }

            return this.Ok(category);
        }
        catch (CategoryServiceException ex)
        {
            // ignored
            this.ModelState.AddModelError("Products", ex.Message);

            return this.BadRequest(this.ModelState);
        }
    }

    /// <summary>
    /// Get all products in a category.
    /// </summary>
    /// <param name="id"></param>
    /// <response code="200">When all the products have been returned.</response>
    /// <response code="404">When the category with the given <paramref name="id"/> does not exist.</response>
    // GET api/categories/{GUID}/products
    [HttpGet("{id}/products")]
    [ProducesResponseType(typeof(IEnumerable<Product>), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetProducts(Guid id)
    {
        var category = await this.Categories.GetCategory(id);

        if (category == null)
        {
            return this.NotFound();
        }

        var productIds = category.Products.Select(p => p.Id);

        var products = await this.Products.GetProducts(productIds);

        return this.Ok(products);
    }
}