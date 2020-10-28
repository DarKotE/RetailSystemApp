using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using RSADataManager.Library.DataAccess;
using RSADataManager.Library.Models;


namespace RSAWebServer.Controllers
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
