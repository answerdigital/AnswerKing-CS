using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AnswerKing.Core.Entities;
using AnswerKing.Repositories.Interfaces;
using Dapper;

namespace AnswerKing.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly IConnectionFactory _connectionFactory;

        public OrderRepository(IConnectionFactory connectionFactory)
        {
            this._connectionFactory = connectionFactory;
        }

        public async Task<List<OrderEntity>> GetAll()
        {
            const string query = @"SELECT O.Id, OS.Status, O.Address,
                                          I.Id, I.Name, I.Price, I.Description, OI.Quantity
                                   FROM [Order] O
                                   LEFT JOIN OrderStatus OS on O.StatusId = OS.Id
                                   LEFT JOIN OrderItem OI on O.Id = OI.OrderId
                                   LEFT JOIN Item I on OI.ItemId = I.Id;";

            using (var connection = this._connectionFactory.GetConnection())
            {
                var orderDictionary = new Dictionary<int, OrderEntity>();
                
                var result = await connection.QueryAsync<OrderEntity, OrderItemEntity, OrderEntity>(
                    query,
                    (order, orderItem) =>
                    {
                        OrderEntity? orderEntry;
                        
                        if (!orderDictionary.TryGetValue(order.Id, out orderEntry))
                        {
                            orderEntry = order;
                            orderEntry.Items = new List<OrderItemEntity>();
                            orderDictionary.Add(orderEntry.Id, orderEntry);
                        }
                        if (orderItem is not null)
                        {
                            orderEntry.Items.Add(orderItem);
                        }

                        return orderEntry;
                    },
                    splitOn: "Id");
                
                return result
                    .Distinct()
                    .ToList();
            }
        }

        public async Task<OrderEntity?> GetById(int id)
        {
            const string query = @"SELECT O.Id, OS.Status, O.Address,
                                          I.Id, I.Name, I.Price, I.Description, OI.Quantity
                                   FROM [Order] O
                                   LEFT JOIN OrderStatus OS on O.StatusId = OS.Id
                                   LEFT JOIN OrderItem OI on O.Id = OI.OrderId
                                   LEFT JOIN Item I on OI.ItemId = I.Id
                                   WHERE O.Id = @Id;";

            using (var connection = this._connectionFactory.GetConnection())
            {
                var orderDictionary = new Dictionary<int, OrderEntity>();
                
                var result = await connection.QueryAsync<OrderEntity, OrderItemEntity, OrderEntity>(
                    query,
                    (order, orderItem) =>
                    {
                        OrderEntity? orderEntry;
                        
                        if (!orderDictionary.TryGetValue(order.Id, out orderEntry))
                        {
                            orderEntry = order;
                            orderEntry.Items = new List<OrderItemEntity>();
                            orderDictionary.Add(orderEntry.Id, orderEntry);
                        }
                        if (orderItem is not null)
                        {
                            orderEntry.Items.Add(orderItem);
                        }

                        return orderEntry;
                    },
                    splitOn: "Id",
                    param: new {Id = id});
                
                return result
                    .Distinct()
                    .FirstOrDefault();
            }
        }

        public async Task<int> Create(OrderEntity orderEntity)
        {
            const string query = @"INSERT INTO [Order] (Address)
                                   OUTPUT INSERTED.Id
                                   VALUES (@Address);";
            
            using (var connection = this._connectionFactory.GetConnection())
            {
                var result = await connection.QuerySingleAsync<int>(query, orderEntity);
                return result;
            }
        }

        public async Task<bool> Update(OrderEntity orderEntity)
        {
            const string query = @"UPDATE [Order] SET Address = @Address WHERE Id = @Id;";
            
            using (var connection = this._connectionFactory.GetConnection())
            {
                var result = await connection.ExecuteAsync(query, orderEntity);
                return result > 0;
            }
        }

        public async Task<bool> Delete(int id)
        {
            const string query = @"DELETE C FROM [Order] as C WHERE C.Id = @Id;";
            
            using (var connection = this._connectionFactory.GetConnection())
            {
                var result = await connection.ExecuteAsync(query, new {Id = id});
                return result > 0;
            }
        }
        
        public async Task<bool> AddItem(int orderId, int itemId)
        {
            const string query = @"INSERT INTO OrderItem (OrderId, ItemId) 
                                   VALUES (@OrderId, @ItemId);";
            
            using (var connection = this._connectionFactory.GetConnection())
            {
                var result = await connection.ExecuteAsync(query, new {OrderId = orderId, ItemId = itemId});
                return result > 0;
            }
        }

        public async Task<bool> RemoveItem(int orderId, int itemId)
        {
            const string query = @"DELETE OI FROM OrderItem as OI
                                   WHERE OI.OrderId = @OrderId
                                   AND OI.ItemId = @ItemId;";
            
            using (var connection = this._connectionFactory.GetConnection())
            {
                var result = await connection.ExecuteAsync(query, new {OrderId = orderId, ItemId = itemId});
                return result > 0;
            }
        }

        public async Task<bool> UpdateItemQuantity(int orderId, int itemId, int quantity)
        {
            const string query = @"UPDATE OrderItem
                                   SET Quantity = @Quantity
                                   WHERE OrderId = @OrderId
                                   AND ItemId = @ItemId;";
            
            using (var connection = this._connectionFactory.GetConnection())
            {
                var result = await connection.ExecuteAsync(
                    query, 
                    new
                    {
                        OrderId = orderId,
                        ItemId = itemId,
                        Quantity = quantity
                    });
                return result > 0;
            }
        }
    }
}