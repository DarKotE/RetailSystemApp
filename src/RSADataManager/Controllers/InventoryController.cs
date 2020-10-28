using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using RSADataManager.Library.DataAccess;
using RSADataManager.Library.Models;

namespace RSADataManager.Controllers
{
    [Authorize]
    public class InventoryController : ApiController
    {
        [Authorize(Roles = "Manager, Admin")]
        public List<InventoryModel> Get()
        {
            var data = new InventoryData();
            return data.GetInventory();
        }

        [Authorize(Roles = "Admin")]
        public void Post(InventoryModel item)
        {
            var data = new InventoryData();
            data.SaveInventoryRecord(item);
        }

    }
}
