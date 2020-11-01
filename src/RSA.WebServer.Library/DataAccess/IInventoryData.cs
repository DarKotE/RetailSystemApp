using System.Collections.Generic;
using RSA.WebServer.Library.Models;

namespace RSA.WebServer.Library.DataAccess
{
    public interface IInventoryData
    {
        List<InventoryModel> GetInventory();
        void SaveInventoryRecord(InventoryModel inventoryRecord);
    }
}