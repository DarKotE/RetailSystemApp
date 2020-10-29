using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using RSA.WebServer.Library.DataAccess;
using RSA.WebServer.Library.Models;

namespace RSA.WebServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class InventoryController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public InventoryController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        [HttpGet]
        [Authorize(Roles = "Manager, Admin")]
        public List<InventoryModel> Get()
        {
            var data = new InventoryData(_configuration);
            return data.GetInventory();
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public void Post(InventoryModel item)
        {
            var data = new InventoryData(_configuration);
            data.SaveInventoryRecord(item);
        }

    }
}
