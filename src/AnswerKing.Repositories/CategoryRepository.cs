using AnswerKing.Core;
using AnswerKing.Core.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnswerKing.Repositories.Interfaces;
using Dapper;

namespace AnswerKing.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly IConnectionFactory _connectionFactory;

        public CategoryRepository(IConnectionFactory connectionFactory)
        {
            this._connectionFactory = connectionFactory;
        }

        public async Task<List<CategoryEntity>> GetAll()
        {
            const string query = @"SELECT C.Id, C.Name FROM Category C;";
            
            using (var connection = this._connectionFactory.GetConnection())
            {
                var result = await connection.QueryAsync<CategoryEntity>(query);
                return result.ToList();
            }
        }

        public async Task<CategoryEntity?> GetById(int id)
        {
            const string query = @"SELECT C.Id, C.Name FROM Category c WHERE c.Id = @Id;";
            
            using (var connection = this._connectionFactory.GetConnection())
            {
                var result = await connection.QueryAsync<CategoryEntity>(query, new {Id = id});
                return result.FirstOrDefault();
            }
        }

        public async Task<int?> Create(CategoryEntity categoryEntity)
        {
            const string existsQuery = @"SELECT COUNT(1) FROM Category WHERE Name = @Name;";
            const string insertQuery = @"INSERT INTO Category (Name) OUTPUT INSERTED.Id VALUES(@Name);";
            
            using (var connection = this._connectionFactory.GetConnection())
            {
                var categoryExists = await connection.QuerySingleAsync<bool>(existsQuery, categoryEntity);
                if (categoryExists)
                {
                    return null;
                }
                
                var result = await connection.QuerySingleAsync<int>(insertQuery, categoryEntity);
                return result;
            }
        }

        public async Task<bool> Update(CategoryEntity categoryEntity)
        {
            const string query = @"UPDATE Category SET Name = @Name WHERE Id = @Id;";
            
            using (var connection = this._connectionFactory.GetConnection())
            {
                var result = await connection.ExecuteAsync(query, categoryEntity);
                return result > 0;
            }
        }

        public async Task<bool> Delete(int id)
        {
            const string query = @"DELETE C FROM Category as C WHERE C.Id = @Id;";
            
            using (var connection = this._connectionFactory.GetConnection())
            {
                var result = await connection.ExecuteAsync(query, new {Id = id});
                return result > 0;
            }
        }
    }
}
