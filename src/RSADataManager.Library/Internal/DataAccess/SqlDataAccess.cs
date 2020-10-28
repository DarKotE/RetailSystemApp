using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Dapper;

namespace RSADataManager.Library.Internal.DataAccess
{
    internal class SqlDataAccess: IDisposable
    {
        private IDbConnection _connection;
        private IDbTransaction _transaction;
        private readonly IConfiguration _configuration;

        public SqlDataAccess(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        public string GetConnectionString(string name)
        {
            return _configuration.GetConnectionString(name);
        }

        public List<T> LoadData<T,U>(string storedProcedure, U parameters, string connectionStringName)
        {
            using (IDbConnection connection = new SqlConnection(GetConnectionString(connectionStringName)))
            {
                return connection
                    .Query<T>(storedProcedure, parameters, commandType: CommandType.StoredProcedure)
                    .ToList();
            }
        }

        public void SaveData<T>(string storedProcedure, T parameters, string connectionStringName)
        {
            using (IDbConnection connection = new SqlConnection(GetConnectionString(connectionStringName)))
            {
                connection
                    .Execute(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
            }
        }

        public void StartTransaction(string connectionStringName)
        {
            _connection = new SqlConnection(GetConnectionString(connectionStringName));
            _connection.Open();
            _transaction = _connection.BeginTransaction();
        }
        public void CommitTransaction()
        {
            _transaction?.Commit();
            _transaction = null;
            _connection?.Close();
            _connection = null;
        }
        public void RollbackTransaction()
        {
            _transaction.Rollback();
            _transaction = null;
            _connection?.Close();
            _connection = null;
        }
        public void SaveDataInTransaction<T>(string storedProcedure, T parameters) => 
            _connection
                .Execute(storedProcedure,
                         parameters,
                         commandType: CommandType.StoredProcedure,
                         transaction: _transaction);
        public List<T> LoadDataInTransaction<T, U>(string storedProcedure, U parameters) => 
            _connection
                    .Query<T>(storedProcedure,
                              parameters,
                              commandType: CommandType.StoredProcedure,
                              transaction: _transaction)
                    .ToList();

        public void Dispose()
        {
            CommitTransaction();
        }
    }
}
