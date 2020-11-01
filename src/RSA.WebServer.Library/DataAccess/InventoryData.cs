using System.Collections.Generic;
using RSA.WebServer.Library.Internal.DataAccess;
using RSA.WebServer.Library.Models;

namespace RSA.WebServer.Library.DataAccess
{
    public class InventoryData : IInventoryData
    {
        private readonly ISqlDataAccess _sql;

        public InventoryData(ISqlDataAccess sql)
        {
            _sql = sql;
        }

        public List<InventoryModel> GetInventory()
        {
            return _sql
                    .LoadData<InventoryModel, dynamic>(storedProcedure: "dbo.spInventory_GetAll",
                                                        parameters: new { },
                                                        connectionStringName: "RSAData");
        }
        public void SaveInventoryRecord(InventoryModel inventoryRecord)
        {
            _sql.SaveData(storedProcedure: "[dbo].[spInventory_Insert]",
                         parameters: inventoryRecord,
                         connectionStringName: "RSAData");
        }
    }
}
