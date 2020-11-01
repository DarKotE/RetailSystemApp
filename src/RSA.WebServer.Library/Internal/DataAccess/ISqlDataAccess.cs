using System;
using System.Collections.Generic;

namespace RSA.WebServer.Library.Internal.DataAccess
{
    public interface ISqlDataAccess
    {
        //string GetConnectionString(string name);
        List<T> LoadData<T,TU>(string storedProcedure, TU parameters, string connectionStringName);
        void SaveData<T>(string storedProcedure, T parameters, string connectionStringName);
        void StartTransaction(string connectionStringName);
        void CommitTransaction();
        void RollbackTransaction();
        void SaveDataInTransaction<T>(string storedProcedure, T parameters);
        List<T> LoadDataInTransaction<T, TU>(string storedProcedure, TU parameters);
    }
}