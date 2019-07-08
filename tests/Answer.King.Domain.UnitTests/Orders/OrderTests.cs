using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Answer.King.Domain.Orders;
using Answer.King.Domain.Orders.Models;
using Answer.King.Test.Common.CustomTraits;
using Xunit;

namespace Answer.King.Domain.UnitTests.Orders
{
    [TestCategory(TestType.Unit)]
    public class OrderTests
    {
        [Fact]
        public void OrderStateStateEnum_MapsToCorrectInt()
        {
            var totalStreamNamesTested = new OrderStateConstantsData().Count();
            var totalConstants = GetAll().Count();

            Assert.Equal(totalStreamNamesTested, totalConstants);
        }

        #region Constructor

        [Fact]
        public void Constructor_NoArguments_CreatesNewEmptyOrder()
        {
            var order = new Order();
            var now = DateTime.UtcNow;

            Assert.NotEqual(Guid.Empty, order.Id);
            Assert.NotEqual(DateTime.MinValue, order.CreatedOn);
            Assert.InRange(order.CreatedOn, now.AddSeconds(-1), now);
            Assert.Equal(order.CreatedOn, order.LastUpdated);
            Assert.Equal(OrderStatus.Created, order.OrderStatus);
        }

        #endregion

        #region AddLineItem

        [Fact]
        public void AddLineItem_OrderStatusCompleted_ThrowsOrderLifecycleException()
        {
            // Arrange
            var order = new Order();
            var id = Guid.NewGuid();
            var name = "name";
            var description = "description";
            var category = new Category(Guid.NewGuid(), "name", "description");
            var price = 1.24;
            var quantity = 2;

            order.CompleteOrder();

            // Act / Assert
            Assert.Throws<OrderLifeCycleException>(() =>
                order.AddLineItem(id, name, description, price, category, quantity));
        }

        [Fact]
        public void AddLineItem_OrderStatusCancelled_ThrowsOrderLifecycleException()
        {
            // Arrange
            var order = new Order();
            var id = Guid.NewGuid();
            var name = "name";
            var description = "description";
            var category = new Category(Guid.NewGuid(), "name", "description");
            var price = 1.24;
            var quantity = 2;

            order.CancelOrder();

            // Act / Assert
            Assert.Throws<OrderLifeCycleException>(() =>
                order.AddLineItem(id, name, description, price, category, quantity));
        }

        [Fact]
        public void AddLineItem_ValidArgumentsWithNewItem_AddsToLineItemsWithCorrectQuantity()
        {
            // Arrange
            var order = new Order();
            var id = Guid.NewGuid();
            var name = "name";
            var description = "description";
            var category = new Category(Guid.NewGuid(), "name", "description");
            var price = 1.24;
            var quantity = 2;

            // Act
            order.AddLineItem(id, name, description, price, category, quantity);

            var lineItem = order.LineItems.FirstOrDefault();

            // Assert
            Assert.NotNull(lineItem);
            Assert.Equal(lineItem.Quantity, quantity);
            Assert.NotNull(lineItem.Product);
            Assert.Equal(lineItem.Product.Id, id);
            Assert.Equal(lineItem.Product.Price, price);
        }

        [Fact]
        public void AddLineItem_ValidArgumentsWithNewItem_AddsToLineItemsAndCalculatesTheCorrectSubtotal()
        {
            // Arrange
            var order = new Order();
            var id = Guid.NewGuid();
            var name = "name";
            var description = "description";
            var category = new Category(Guid.NewGuid(), "name", "description");
            var price = 1.24;
            var quantity = 2;

            // Act
            order.AddLineItem(id, name, description, price, category, quantity);

            var lineItem = order.LineItems.FirstOrDefault();

            // Assert
            Assert.NotNull(lineItem);
            Assert.Equal(lineItem.SubTotal, quantity * price);
        }

        [Fact]
        public void AddLineItem_ValidArgumentsWithNewItem_AddsToLineItemsWithCorrectPrice()
        {
            // Arrange
            var order = new Order();
            var id = Guid.NewGuid();
            var name = "name";
            var description = "description";
            var category = new Category(Guid.NewGuid(), "name", "description");
            var price = 1.24;
            var quantity = 2;

            // Act
            order.AddLineItem(id, name, description, price, category, quantity);

            var lineItem = order.LineItems.FirstOrDefault();

            // Assert
            Assert.NotNull(lineItem);
            Assert.NotNull(lineItem.Product);
            Assert.Equal(lineItem.Product.Price, price);
        }

        [Fact]
        public void AddLineItem_ValidArgumentsWithExistingLineItem_UpdatesExistingLineItemQuantity()
        {
            // Arrange
            var order = new Order();
            var id = Guid.NewGuid();
            var name = "name";
            var description = "description";
            var category = new Category(Guid.NewGuid(), "name", "description");
            var price = 1.24;
            var quantity = 2;

            var expectedQuantity = 10;

            // Act
            order.AddLineItem(id, name, description, price, category, quantity);
            order.AddLineItem(id, name, description, price, category, 8);

            var lineItem = order.LineItems.FirstOrDefault();

            // Assert
            Assert.NotNull(lineItem);
            Assert.NotNull(lineItem.Product);
            Assert.Equal(lineItem.Product.Name, name);
            Assert.Equal(lineItem.Product.Description, description);
            Assert.Equal(lineItem.Product.Category, category);
            Assert.Equal(lineItem.Product.Price, price);
            Assert.Equal(lineItem.Quantity, expectedQuantity);
        }

        #endregion AddLineItem

        #region RemoveLineItem

        [Fact]
        public void RemoveLineItem_OrderStatusCompleted_ThrowsOrderLifecycleException()
        {
            // Arrange
            var order = new Order();
            var id = Guid.NewGuid();
            var quantity = 2;

            order.CompleteOrder();

            // Act / Assert
            Assert.Throws<OrderLifeCycleException>(() =>
                order.RemoveLineItem(id, quantity));
        }

        [Fact]
        public void RemoveLineItem_OrderStatusCancel_ThrowsOrderLifecycleException()
        {
            // Arrange
            var order = new Order();
            var id = Guid.NewGuid();
            var quantity = 2;

            order.CancelOrder();

            // Act / Assert
            Assert.Throws<OrderLifeCycleException>(() =>
                order.RemoveLineItem(id, quantity));
        }

        [Fact]
        public void RemoveLineItem_LineItemDoesNotExistInOrder_DoesNotAttemptToRemoveFromOrder()
        {
            // Arrange
            var order = new Order();
            var id = Guid.NewGuid();
            var quantity = 3;

            // Act
            order.RemoveLineItem(id, quantity);

            // Assert
            Assert.Empty(order.LineItems);
        }

        [Fact]
        public void RemoveLineItem_LineItemExistsInOrder_DecrementCorrectQuantityValue()
        {
            // Arrange
            var order = new Order();
            var id = Guid.NewGuid();
            var productName = "PRODUCT_NAME";
            var productDescription = "PRODUCT_DESCRIPTION";
            var category = new Category(
                Guid.Parse("9BAC7050-D3C4-4793-A5C8-BEEAC4EA4229"),
                "CATEGORY_NAME",
                "CATEGORY_DESCRIPTION"
            );
            var quantity = 5;
            var price = 1.25;

            order.AddLineItem(id, productName, productDescription, price, category, quantity);

            // Act
            order.RemoveLineItem(id, 3);

            var lineItem = order.LineItems.FirstOrDefault();

            // Assert
            Assert.NotNull(lineItem);
            Assert.Equal(2, lineItem.Quantity);
        }

        [Fact]
        public void RemoveLineItem_LineItemExistsInOrder_RemovedFromOrderIfQuantityGteCurrent()
        {
            // Arrange
            var order = new Order();
            var id = Guid.NewGuid();
            var productName = "PRODUCT_NAME";
            var productDescription = "PRODUCT_DESCRIPTION";
            var category = new Category(
                Guid.Parse("9BAC7050-D3C4-4793-A5C8-BEEAC4EA4229"),
                "CATEGORY_NAME",
                "CATEGORY_DESCRIPTION"
            );
            var quantity = 3;
            var price = 1.25;

            order.AddLineItem(id, productName, productDescription, price, category, quantity);

            // Act
            order.RemoveLineItem(id, 3);

            var lineItem = order.LineItems.FirstOrDefault();

            // Assert
            Assert.Null(lineItem);
            Assert.Equal(0, order.LineItems.Count);
        }

        #endregion RemoveLineItem

        #region CompleteOrder

        [Fact]
        public void CompleteOrder_OrderStatusCancelled_ThrowsOrderLifecycleException()
        {
            var order = new Order();
            order.CancelOrder();

            Assert.Throws<OrderLifeCycleException>(() => order.CompleteOrder());
        }

        #endregion

        #region CancelOrder

        [Fact]
        public void CancelOrder_OrderStatusCompleted_ThrowsOrderLifecycleException()
        {
            var order = new Order();
            order.CompleteOrder();

            Assert.Throws<OrderLifeCycleException>(() => order.CancelOrder());
        }

        #endregion

        #region OrderTotal

        [Fact]
        public void OrderTotal_WhenOrderHasLineItems_ReturnsSumOfOrdersLineItemPrice()
        {
            // Arrange
            var order = new Order();
            var category = new Category(Guid.NewGuid(), "name", "description");

            var itemPrice1 = 1.50;
            var itemPrice2 = 4.75;

            var itemQuantity1 = 2;
            var itemQuantity2 = 6;

            var expectedTotal = itemPrice1 * itemQuantity1 + itemPrice2 * itemQuantity2;

            // Act
            order.AddLineItem(Guid.NewGuid(), "name1", "description1", 1.50, category, 2);
            order.AddLineItem(Guid.NewGuid(), "name2", "description2", 4.75, category, 6);

            Assert.Equal(expectedTotal, order.OrderTotal);
        }

        #endregion

        #region Helpers

        private static IEnumerable<string> GetAll()
        {
            var enumValues = Enum.GetNames(typeof(OrderStatus));

            return enumValues;
        }

        #endregion Helpers
    }

    #region ClassData

    public class OrderStateConstantsData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            return new List<object[]>
            {
                new object[] { 0, OrderStatus.Created },
                new object[] { 1, OrderStatus.Paid },
                new object[] { 2, OrderStatus.Cancelled },
            }.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }

    #endregion ClassData
}