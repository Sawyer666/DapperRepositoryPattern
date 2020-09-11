using Dapper;
using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DapperRepositoryPattern
{
    public abstract class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly string _tableName;

        protected GenericRepository(string tableName)
        {
            _tableName = tableName;
        }

        private NpgsqlConnection SqlConnection()
        {
            return new NpgsqlConnection("Server=127.0.0.1; Port=5432; User Id=postgres; Password=greenday; Database=MessageDb;");
        }

        private IDbConnection CreateConnection()
        {
            NpgsqlConnection connection = null;
            try
            {
                connection = SqlConnection();
                connection.Open();
            }
            catch (Exception)
            {
                throw;
            }
            return connection;
        }

        private static List<string> GenerateListOfProperties(IEnumerable<PropertyInfo> listOfProperties)
        {
            return (from prop in listOfProperties
                    let attributes = prop.GetCustomAttributes(typeof(DescriptionAttribute), false)
                    where attributes.Length <= 0 || (attributes[0] as DescriptionAttribute)?.Description != "ignore"
                    select prop.Name).ToList();
        }

        private string GenerateInsertQuery()
        {
            var insertQuery = new StringBuilder($"INSERT INTO {_tableName}").Append("(");

            var properties = GenerateListOfProperties(GetProperties);
            properties.ForEach(prop => { insertQuery.Append($"{prop},"); });

            insertQuery
               .Remove(insertQuery.Length - 1, 1)
               .Append(") VALUES (");

            properties.ForEach(prop => { insertQuery.Append($"@{prop},"); });
            insertQuery
                .Remove(insertQuery.Length - 1, 1)
                .Append(")");

            return insertQuery.ToString();
        }

        protected virtual IEnumerable<PropertyInfo> GetProperties
        {
            get
            {
                return typeof(T).GetProperties();
            }
        }


        public async Task<IEnumerable<T>> GetAllAsync()
        {
            IEnumerable<T> data = null;
            try
            {
                using (var connection = CreateConnection())
                {
                    data = await connection.QueryAsync<T>($"SELECT * FROM {_tableName}");
                }
            }
            catch (Exception)
            { }
            return data;
        }

        public Task DeleteRowAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<T> GetAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task InsertAsync(T t)
        {
            await Task.Factory.StartNew(() =>
           {
               var insertQuery = GenerateInsertQuery();
               NpgsqlTransaction tranc = null;
               using (var connection = CreateConnection())
               {
                   try
                   {
                       tranc = (NpgsqlTransaction)connection.BeginTransaction();
                       for (int r = 0; r < 50000; r++)
                       {
                           connection.Execute(insertQuery, t);
                       }
                       tranc.Commit();
                   }
                   catch (Exception)
                   {
                   }
                   finally
                   {
                       tranc.Dispose();
                   }
               }
           });
        }

        public async Task<int> SaveRangeAsync(IEnumerable<T> data)
        {
            var inserted = 0;
            var query = GenerateInsertQuery();
            //using (var connection = CreateConnection())
            //{
            //    await connection.ExecuteAsync("INSERT INTO messages([message]) VALUES (@message)", new DbRecord("13213"));
            //}
            //  return 0;
            using (var connection = CreateConnection())
            {
                //   {
                try
                {
                    inserted += await connection.ExecuteAsync(query, data);
                    //    tranc.Commit();
                }
                catch (Exception)
                {
                    //   tranc.Rollback();
                }
                //    }
            }
            return inserted;
        }

        public Task UpdateAsync(T t)
        {
            throw new NotImplementedException();
        }
    }
}
