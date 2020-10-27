using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RSADataManager.Library.DataAccess;
using RSADataManager.Library.Models;

namespace RSAWebServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class InventoryController : ControllerBase
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
