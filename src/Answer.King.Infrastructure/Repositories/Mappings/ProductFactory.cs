using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Answer.King.Domain.Repositories.Models;

[assembly: InternalsVisibleTo("Answer.King.Domain.UnitTests")]

namespace Answer.King.Infrastructure.Repositories.Mappings;

internal static class ProductFactory
{
    public static Product CreateProduct(
        long id,
        string name,
        string description,
        double price,
        Category category,
        bool retired)
    {
        var ctor = typeof(Product)
            .GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)
            .SingleOrDefault(c => c.IsPrivate);

        var parameters = new object[] { id, name, description, price, category, retired };

        /* invoking a private constructor will wrap up any exception into a
         * TargetInvocationException so here I unwrap it
         */
        try
        {
            return (Product) ctor?.Invoke(parameters)!;
        }
        catch (TargetInvocationException ex)
        {
            var exception = ex.InnerException ?? ex;
            throw exception;
        }
    }
}
