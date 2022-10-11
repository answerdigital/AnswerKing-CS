using AnswerKing.Core;
using AnswerKing.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AnswerKing.Repositories.Interfaces;
using Dapper;

namespace AnswerKing.Repositories
{
    public class ItemRepository : IItemRepository
    {
        private readonly IConnectionFactory _connectionFactory;

        public ItemRepository(IConnectionFactory connectionFactory)
        {
            this._connectionFactory = connectionFactory;
        }

        public async Task<List<ItemEntity>> GetAll()
        {
            const string query = @"SELECT I.Id, I.Name, I.Price, I.Description, C.Id, C.Name
                                   FROM Item I 
                                   LEFT JOIN ItemCategory IC on I.Id = IC.ItemId
                                   LEFT JOIN Category C on C.Id = IC.CategoryId;";
            
            using (var connection = this._connectionFactory.GetConnection())
            {
                var itemDictionary = new Dictionary<int, ItemEntity>();
                
                var result = await connection.QueryAsync<ItemEntity, CategoryEntity, ItemEntity>(
                    query,
                    (item, category) =>
                    {
                        ItemEntity? itemEntry;
                        
                        if (!itemDictionary.TryGetValue(item.Id, out itemEntry))
                        {
                            itemEntry = item;
                            itemEntry.Categories = new List<CategoryEntity>();
                            itemDictionary.Add(itemEntry.Id, itemEntry);
                        }
                        if (category is not null)
                        {
                            itemEntry.Categories.Add(category);
                        }
                        
                        return itemEntry;
                    },
                    splitOn: "Id");
                
                return result
                    .Distinct()
                    .ToList();
            }
        }
        
        public async Task<List<ItemEntity>> GetByCategory(int categoryId)
        {
            const string query = @"SELECT I.Id, I.Name, I.Price, I.Description, C.Id, C.Name
                                   FROM Item I 
                                   LEFT JOIN ItemCategory IC on I.Id = IC.ItemId
                                   LEFT JOIN Category C on C.Id = IC.CategoryId;";
            
            using (var connection = this._connectionFactory.GetConnection())
            {
                var itemDictionary = new Dictionary<int, ItemEntity>();
                
                var result = await connection.QueryAsync<ItemEntity, CategoryEntity, ItemEntity>(
                    query,
                    (item, category) =>
                    {
                        ItemEntity? itemEntry;

                        if (!itemDictionary.TryGetValue(item.Id, out itemEntry))
                        {
                            itemEntry = item;
                            itemEntry.Categories = new List<CategoryEntity>();
                            itemDictionary.Add(itemEntry.Id, itemEntry);
                        }
                        if (category is not null)
                        {
                            itemEntry.Categories.Add(category);
                        }

                        return itemEntry;
                    },
                    splitOn: "Id",
                    param: new {CategoryId = categoryId});
                
                // TODO: Remove the LINQ where clause and filter with SQL
                // I'm struggling to write an SQL query for selecting all items with a particular category,
                // and include all the other categories of that item.
                
                return result
                    .Distinct()
                    .Where(item => item.Categories.FirstOrDefault(c => c.Id == categoryId) is not null)
                    .ToList();
            }
        }

        public async Task<ItemEntity?> GetById(int id)
        {
            const string query = @"SELECT I.Id, I.Name, I.Price, I.Description, C.Id, C.Name
                                   FROM Item I 
                                   LEFT JOIN ItemCategory IC on I.Id = IC.ItemId
                                   LEFT JOIN Category C on C.Id = IC.CategoryId
                                   WHERE I.Id = @Id;";
            
            using (var connection = this._connectionFactory.GetConnection())
            {
                var itemDictionary = new Dictionary<int, ItemEntity>();
                
                var result = await connection.QueryAsync<ItemEntity, CategoryEntity, ItemEntity>(
                    query,
                    (item, category) =>
                    {
                        ItemEntity? itemEntry;

                        if (!itemDictionary.TryGetValue(item.Id, out itemEntry))
                        {
                            itemEntry = item;
                            itemEntry.Categories = new List<CategoryEntity>();
                            itemDictionary.Add(itemEntry.Id, itemEntry);
                        }
                        if (category is not null)
                        {
                          itemEntry.Categories.Add(category);
                        }

                        return itemEntry;
                    },
                    splitOn: "Id",
                    param: new {Id = id});
                
                return result
                    .Distinct()
                    .FirstOrDefault();
            }
        }

        public async Task<int?> Create(ItemEntity itemEntity)
        {
            const string existsQuery = @"SELECT COUNT(1) FROM Item WHERE Name = @Name";
            const string insertQuery = @"INSERT INTO Item (Name, Price, Description)
                                         OUTPUT INSERTED.Id
                                         VALUES (@Name, @Price, @Description);";
            
            using (var connection = this._connectionFactory.GetConnection())
            {
                var itemExists = await connection.QuerySingleAsync<bool>(existsQuery, itemEntity);
                if (itemExists)
                {
                    return null;
                }
                
                var result = await connection.QuerySingleAsync<int>(insertQuery, itemEntity);
                return result;
            }
        }

        public async Task<bool> Update(ItemEntity itemEntity)
        {
            const string query = @"UPDATE Item 
                                   SET Name = @Name, Price = @Price, Description = @Description
                                   WHERE Id = @Id;";

            using (var connection = this._connectionFactory.GetConnection())
            {
                var result = await connection.ExecuteAsync(query, itemEntity);
                return result > 0;
            }
        }

        public async Task<bool> Delete(int id)
        {
            const string query = @"DELETE I FROM Item as I WHERE I.Id = @Id;";
            
            using (var connection = this._connectionFactory.GetConnection())
            {
                var result = await connection.ExecuteAsync(query, new {Id = id});
                return result > 0;
            }
        }

        public async Task<bool> AddCategory(int itemId, int categoryId)
        {
            const string query = @"INSERT INTO ItemCategory (ItemId, CategoryId) 
                                   VALUES(@ItemId, @CategoryId);";
            
            using (var connection = this._connectionFactory.GetConnection())
            {
                var result = await connection.ExecuteAsync(query, new {ItemId = itemId, CategoryId = categoryId});
                return result > 0;
            }
        }

        public async Task<bool> RemoveCategory(int itemId, int categoryId)
        {
            const string query = @"DELETE IC FROM ItemCategory as IC
                                   WHERE IC.ItemId = @ItemId
                                   AND IC.CategoryId = @CategoryId;";
            
            using (var connection = this._connectionFactory.GetConnection())
            {
                var result = await connection.ExecuteAsync(query, new {ItemId = itemId, CategoryId = categoryId});
                return result > 0;
            }
        }
    }
}
