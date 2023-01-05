using System.Reflection;
using Answer.King.Domain.Inventory;
using Answer.King.Domain.Repositories.Models;
using Answer.King.Infrastructure.Repositories.Mappings;
using Answer.King.Test.Common.CustomTraits;
using Xunit;

namespace Answer.King.Infrastructure.UnitTests.Repositories.Factories;

[TestCategory(TestType.Unit)]
public class PaymentFactoryTests
{
    [Fact]
    public void CreatePayment_ConstructorExists_ReturnsPayment()
    {
        // Arrange / Act
        var result = PaymentFactory.CreatePayment(1, 1, 1, 1, DateTime.UtcNow);

        // Assert
        Assert.IsType<Payment>(result);
    }

    [Fact]
    public void CreatePayment_ConstructorNotFound_ReturnsException()
    {
        // Arrange
        var paymentFactoryConstructorPropertyInfo =
        typeof(PaymentFactory).GetProperty("PaymentConstructor", BindingFlags.Static | BindingFlags.NonPublic);

        var constructor = paymentFactoryConstructorPropertyInfo?.GetValue(null);

        var wrongConstructor = typeof(Category).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)
            .SingleOrDefault(c => c.IsPrivate && c.GetParameters().Length > 0);

        paymentFactoryConstructorPropertyInfo?.SetValue(null, wrongConstructor);

        // Act // Assert
        Assert.Throws<TargetParameterCountException>(() =>
            PaymentFactory.CreatePayment(1, 1, 1, 1, DateTime.UtcNow));

        paymentFactoryConstructorPropertyInfo?.SetValue(null, constructor);
    }
}
