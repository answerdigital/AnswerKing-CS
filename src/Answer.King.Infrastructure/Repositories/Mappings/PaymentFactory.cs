using System;
using System.Linq;
using System.Reflection;
using Answer.King.Domain.Repositories.Models;

namespace Answer.King.Infrastructure.Repositories.Mappings;

internal static class PaymentFactory
{
    public static Payment CreatePayment(long id, long orderId, double amount, double orderTotal, DateTime date)
    {
        var ctor = typeof(Payment)
            .GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)
            .SingleOrDefault(c => c.IsPrivate);

        var parameters = new object[] { id, orderId, amount, orderTotal, date };

        /* invoking a private constructor will wrap up any exception into a
         * TargetInvocationException so here I unwrap it
         */
        try
        {
            return (Payment)ctor?.Invoke(parameters)!;
        }
        catch (TargetInvocationException ex)
        {
            var exception = ex.InnerException ?? ex;
            throw exception;
        }
    }
}
