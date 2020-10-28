using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RSADataManager.Library.Internal.DataAccess;
using RSADataManager.Library.Models;

namespace RSADataManager.Library.DataAccess
{
    public class InventoryData
    {
        public List<InventoryModel> GetInventory()
        {
            var sql = new SqlDataAccess();
            return sql
                    .LoadData<InventoryModel, dynamic>(storedProcedure: "dbo.spInventory_GetAll",
                                                        parameters: new { },
                                                        connectionStringName: "RSAData");
        }
        public void SaveInventoryRecord(InventoryModel inventoryRecord)
        {
            var sql = new SqlDataAccess();
            sql.SaveData(storedProcedure: "[dbo].[spInventory_Insert]",
                         parameters: inventoryRecord,
                         connectionStringName: "RSAData");
        }
    }
}
