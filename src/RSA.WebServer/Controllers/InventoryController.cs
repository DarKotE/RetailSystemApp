using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RSA.WebServer.Library.DataAccess;
using RSA.WebServer.Library.Models;

namespace RSA.WebServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryData _inventoryData;

        public InventoryController(IInventoryData inventoryData)
        {
            _inventoryData = inventoryData;
        }

        [HttpGet]
        [Authorize(Roles = "Manager, Admin")]
        public List<InventoryModel> Get()
        {
            return _inventoryData.GetInventory();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public void Post(InventoryModel item)
        {
            _inventoryData.SaveInventoryRecord(item);
        }

    }
}
