using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using RSA.WebServer.Library.Helpers;
using RSA.WebServer.Library.Internal.DataAccess;
using RSA.WebServer.Library.Models;

namespace RSA.WebServer.Library.DataAccess
{
    public class SaleData
    {
        private readonly IConfiguration _configuration;

        public SaleData(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        //TODO remove biz-logic
        public void SaveSale(SaleModel saleInfo, string cashierId)
        {
            var details = new List<SaleDetailDBModel>();
            var products = new ProductData(_configuration);
            var taxRate = ConfigHelper.GetTaxRate();
            foreach (var item in saleInfo.SaleDetails)
            {
                var detail = new SaleDetailDBModel
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity
                };
                var productInfo = products.GetProductById(detail.ProductId);
                if (products is null)
                {
                    throw new Exception($"The product Id of {detail.ProductId} not found in the Db");
                }
                detail.PurchasePrice = productInfo.RetailPrice * detail.Quantity;
                if (productInfo.IsTaxable)
                {
                    detail.Tax = detail.PurchasePrice * taxRate / 100;
                }
                details.Add(detail);
            }
            var sale = new SaleDBModel
            {
                SubTotal = details.Sum(x => x.PurchasePrice),
                Tax = details.Sum(x => x.Tax),
                CashierId = cashierId
            };
            sale.Total = sale.SubTotal + sale.Tax;

            using var sql = new SqlDataAccess(_configuration);
            try
            {
                sql.StartTransaction("RSAData");
                sql.SaveDataInTransaction("dbo.spSale_Insert", sale);
                sale.Id = sql
                    .LoadDataInTransaction<int, dynamic>("spSale_Lookup", new {sale.CashierId, sale.SaleDate })
                    .FirstOrDefault();
                foreach (var item in details)
                {
                    item.SaleId = sale.Id;
                    sql.SaveDataInTransaction("dbo.spSaleDetail_Insert", item);
                }
                sql.CommitTransaction();
            }
            catch
            {
                sql.RollbackTransaction();
                throw;
            }
        }
        public List<SaleReportModel> GetSaleReport()
        {
            var sql = new SqlDataAccess(_configuration);
            return sql
                    .LoadData<SaleReportModel, dynamic>(storedProcedure: "dbo.spSale_SaleReport",
                                                        parameters: new { },
                                                        connectionStringName: "RSAData");
        }
    }
}
