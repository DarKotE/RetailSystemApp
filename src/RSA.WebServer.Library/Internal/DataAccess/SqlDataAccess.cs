using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace RSA.WebServer.Library.Internal.DataAccess
{
    public class SqlDataAccess: ISqlDataAccess, IDisposable
    {
        private IDbConnection _connection;
        private IDbTransaction _transaction;
        private readonly IConfiguration _configuration;
        private readonly ILogger<SqlDataAccess> _logger;

        public SqlDataAccess(IConfiguration configuration, ILogger<SqlDataAccess> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        private string GetConnectionString(string name)
        {
            return _configuration.GetConnectionString(name);
        }

        public List<T> LoadData<T,TU>(string storedProcedure, TU parameters, string connectionStringName)
        {
            using IDbConnection connection = new SqlConnection(GetConnectionString(connectionStringName));
            return connection
                .Query<T>(storedProcedure, parameters, commandType: CommandType.StoredProcedure)
                .ToList();
        }

        public void SaveData<T>(string storedProcedure, T parameters, string connectionStringName)
        {
            using IDbConnection connection = new SqlConnection(GetConnectionString(connectionStringName));
            connection
                .Execute(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
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
            _transaction?.Dispose(); ////TODO CHECK
            //_transaction = null;
            _connection?.Close();
            _connection?.Dispose();
            //_connection = null;
        }
        public void RollbackTransaction()
        {
            _transaction?.Rollback();
            _transaction?.Dispose(); ////TODO CHECK
            //_transaction = null;
            _connection?.Close();
            //_connection = null;
            _logger.LogError("Transaction failed, rollback was called");
        }
        public void SaveDataInTransaction<T>(string storedProcedure, T parameters) =>
            _connection
                .Execute(storedProcedure,
                         parameters,
                         commandType: CommandType.StoredProcedure,
                         transaction: _transaction);
        public List<T> LoadDataInTransaction<T, TU>(string storedProcedure, TU parameters) =>
            _connection
                    .Query<T>(storedProcedure,
                              parameters,
                              commandType: CommandType.StoredProcedure,
                              transaction: _transaction)
                    .ToList();

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual void Dispose(bool disposing)
        {
            try
            {
                CommitTransaction();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Commit transaction failed in the dispose method: {ex.Message}");
            }
        }


    }
}
