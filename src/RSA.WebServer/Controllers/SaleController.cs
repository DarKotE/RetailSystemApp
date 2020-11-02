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
        private readonly ISaleData _saleData;

        public SaleController(ISaleData saleData)
        {
            _saleData = saleData;
        }
        [HttpPost]
        [Authorize(Roles = "Cashier")]
        public void Post(SaleModel sale)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            _saleData.SaveSale(sale, userId);
        }
        [HttpGet]
        [Authorize(Roles = "Manager, Admin")]
        [Route("GetSalesReport")]
        public List<SaleReportModel> GetSaleReport()
        {
            return _saleData.GetSaleReport();
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("GetTaxRate")]
        public decimal GetTaxRate()
        {
            return _saleData.GetTaxRate();
        }
    }
}
