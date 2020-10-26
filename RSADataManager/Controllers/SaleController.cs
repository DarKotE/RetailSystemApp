using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using RSADataManager.Library.DataAccess;
using RSADataManager.Library.Models;
using RSADataManager.Models;

namespace RSADataManager.Controllers
{
    [Authorize]
    public class SaleController : ApiController
    {
        public void Post(SaleModel sale)
        {
            var data = new SaleData();
            string userId = RequestContext.Principal.Identity.GetUserId();
            data.SaveSale(sale, userId);
        }

        [Route("GetSalesReport")]
        public List<SaleReportModel> GetSaleReport()
        {
            var data = new SaleData();
            return data.GetSaleReport();
        }
    }
}
