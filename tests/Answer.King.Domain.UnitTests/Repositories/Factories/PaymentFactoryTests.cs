using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Answer.King.Domain.Inventory;
using Answer.King.Domain.Repositories.Models;
using Answer.King.Infrastructure.Repositories.Mappings;
using Answer.King.Test.Common.CustomTraits;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace Answer.King.Domain.UnitTests.Repositories.Factories;

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
        var PaymentFactoryConstructorFieldInfo =
        typeof(PaymentFactory).GetField($"<PaymentConstructor>k__BackingField", BindingFlags.Static | BindingFlags.NonPublic);

        var constructor = PaymentFactoryConstructorFieldInfo?.GetValue(null);

        var wrongConstructor = typeof(Category).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)
            .SingleOrDefault(c => c.IsPrivate && c.GetParameters().Length > 0);

        PaymentFactoryConstructorFieldInfo?.SetValue(null, wrongConstructor);

        // Act // Assert
        Assert.Throws<TargetParameterCountException>(() =>
            PaymentFactory.CreatePayment(1, 1, 1, 1, DateTime.UtcNow));

        PaymentFactoryConstructorFieldInfo?.SetValue(null, constructor);
    }


}

