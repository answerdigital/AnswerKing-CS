using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Answer.King.Domain.Repositories.Models;

[assembly: InternalsVisibleTo("Answer.King.Domain.UnitTests")]

namespace Answer.King.Infrastructure.Repositories.Mappings;

internal static class ProductFactory
{
    private static ConstructorInfo? ProductConstructor { get; set; } = typeof(Product)
        .GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)
        .SingleOrDefault(c => c.IsPrivate && c.GetParameters().Length > 0);

    public static Product CreateProduct(
        long id,
        string name,
        string description,
        double price,
        IList<CategoryId> categories,
        IList<TagId> tags,
        bool retired)
    {
        var parameters = new object[] { id, name, description, price, categories, tags, retired };

        /* invoking a private constructor will wrap up any exception into a
         * TargetInvocationException so here I unwrap it
         */
        try
        {
            return (Product)ProductConstructor?.Invoke(parameters)!;
        }
        catch (TargetInvocationException ex)
        {
            var exception = ex.InnerException ?? ex;
            throw exception;
        }
    }
}
