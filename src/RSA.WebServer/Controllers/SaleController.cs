using System.Collections.Generic;
using System.Security.Claims;
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
    public class SaleController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public SaleController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        [HttpPost]
        [Authorize(Roles = "Cashier")]
        public void Post(SaleModel sale)
        {
            var data = new SaleData(_configuration);
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            data.SaveSale(sale, userId);
        }
        [HttpGet]
        [Authorize(Roles = "Manager, Admin")]
        [Route("GetSalesReport")]
        public List<SaleReportModel> GetSaleReport()
        {
            var data = new SaleData(_configuration);
            return data.GetSaleReport();
        }
    }
}
