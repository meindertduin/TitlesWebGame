using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using TitlesWebGame.Domain.Interfaces;

namespace TitlesWebGame.Api.Infrastructure.Repositories
{
    public abstract class Repository<T> : IRepository<T>
    {
        private readonly string _tableName;
        private readonly IConfiguration _configuration;

        protected Repository(string tableName, IConfiguration configuration)
        {
            _tableName = tableName;
            _configuration = configuration;
        }

        private SqliteConnection SqliteConnection()
        {
            return new SqliteConnection(_configuration["ConnectionString"]);
        }

        private IDbConnection CreateConnection()
        {
            var connection = SqliteConnection();
            connection.Open();
            return connection;
        }

        private IEnumerable<PropertyInfo> GetProperties => typeof(T).GetProperties();
        
        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            using var connection  = CreateConnection();
            return await connection.QueryAsync<T>($"SELECT * FROM {_tableName}");
        }

        public virtual async Task DeleteRowAsync(int id)
        {
            using var connection = CreateConnection();
            await connection.ExecuteAsync($"DELETE FROM {_tableName} WHERE Id=@Id", new {Id = id});
        }

        public virtual async Task<T> GetAsync(int id)
        {
            using var connection = CreateConnection();
            var result =
                await connection.QuerySingleOrDefaultAsync<T>($"SELECT * FROM {_tableName} WHERE Id=@Id",
                    new {id = id});
            return result;
        }

        public virtual async Task<int> SaveRangeAsync(IEnumerable<T> list)
        {
            var inserted = 0;
            var query = GenerateInsertQuery();
            using var connection = CreateConnection();
            inserted += await connection.ExecuteAsync(query, list);

            return inserted;
        }
        
        public virtual async Task InsertAsync(T t)
        {
            var insertQuery = GenerateInsertQuery();
            using var connection = CreateConnection();
            await connection.ExecuteAsync(insertQuery, t);
        }

        private string GenerateInsertQuery()
        {
            var insertQuery = new StringBuilder($"INSERT INTO {_tableName} ");

            insertQuery.Append("(");

            var properties = GenerateListOfProperties(GetProperties);
            properties.ForEach(prop => { insertQuery.Append($"[{prop}],"); });

            insertQuery
                .Remove(insertQuery.Length - 1, 1)
                .Append(") VALUES (");
            
            properties.ForEach(prop => { insertQuery.Append($"@{prop},"); });
            
            insertQuery
                .Remove(insertQuery.Length - 1, 1)
                .Append(")");
            
            return insertQuery.ToString();
        }

        private List<string> GenerateListOfProperties(IEnumerable<PropertyInfo> propertiesList)
        {
            return (from prop in propertiesList let attributes = prop.GetCustomAttributes(typeof(DescriptionAttribute), false)
                where attributes.Length <= 0 || (attributes[0] as DescriptionAttribute)?.Description != "ignore" select prop.Name).ToList();
        }

        public virtual async Task UpdateAsync(T t)
        {
            var updateQuery = GenerateUpdateQuery();

            using var connection = CreateConnection();
            await connection.ExecuteAsync(updateQuery, t);
        }
        
        private string GenerateUpdateQuery()
        {
            var updateQuery = new StringBuilder($"UPDATE {_tableName} SET ");
            var properties = GenerateListOfProperties(GetProperties);

            properties.ForEach(property =>
            {
                if (!property.Equals("Id"))
                {
                    updateQuery.Append($"{property}=@{property},");
                }
            });

            updateQuery.Remove(updateQuery.Length - 1, 1); //remove last comma
            updateQuery.Append(" WHERE Id=@Id");

            return updateQuery.ToString();
        }
    }
}