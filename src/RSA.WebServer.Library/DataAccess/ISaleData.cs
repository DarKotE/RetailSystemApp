using System.Collections.Generic;
using RSA.WebServer.Library.Models;

namespace RSA.WebServer.Library.DataAccess
{
    public interface ISaleData
    {
        void SaveSale(SaleModel saleInfo, string cashierId);
        List<SaleReportModel> GetSaleReport();
    }
}