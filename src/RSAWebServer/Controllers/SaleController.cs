using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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
    public class SaleController : ControllerBase
    {
        [Authorize(Roles = "Cashier")]
        public void Post(SaleModel sale)
        {
            var data = new SaleData();
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            data.SaveSale(sale, userId);
        }
        [Authorize(Roles = "Manager, Admin")]
        [Route("GetSalesReport")]
        public List<SaleReportModel> GetSaleReport()
        {
            var data = new SaleData();
            return data.GetSaleReport();
        }
    }
}
