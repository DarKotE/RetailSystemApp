using System;
using System.Collections.Generic;
using System.Linq;
using RSA.WebServer.Library.Helpers;
using RSA.WebServer.Library.Internal.DataAccess;
using RSA.WebServer.Library.Models;

namespace RSA.WebServer.Library.DataAccess
{
    public class SaleData : ISaleData
    {

        private readonly IProductData _productData;
        private readonly ISqlDataAccess _sql;

        public SaleData(IProductData productData,ISqlDataAccess sql)
        {
            _productData = productData;
            _sql = sql;
        }

        //TODO remove biz-logic
        public void SaveSale(SaleModel saleInfo, string cashierId)
        {
            var details = new List<SaleDetailDbModel>();
            var taxRate = ConfigHelper.GetTaxRate();
            foreach (var item in saleInfo.SaleDetails)
            {
                var detail = new SaleDetailDbModel
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity
                };
                var productInfo = _productData.GetProductById(detail.ProductId);
                if (_productData is null)
                {
                    // TODO if always false
                    throw new Exception($"The product Id of {detail.ProductId} not found in the Db");
                }
                detail.PurchasePrice = productInfo.RetailPrice * detail.Quantity;
                if (productInfo.IsTaxable)
                {
                    detail.Tax = detail.PurchasePrice * taxRate / 100;
                }
                details.Add(detail);
            }
            var sale = new SaleDbModel
            {
                SubTotal = details.Sum(x => x.PurchasePrice),
                Tax = details.Sum(x => x.Tax),
                CashierId = cashierId
            };
            sale.Total = sale.SubTotal + sale.Tax;

            try
            {
                _sql.StartTransaction("RSAData");
                _sql.SaveDataInTransaction("dbo.spSale_Insert", sale);
                sale.Id = _sql
                    .LoadDataInTransaction<int, dynamic>("spSale_Lookup", new {sale.CashierId, sale.SaleDate })
                    .FirstOrDefault();
                foreach (var item in details)
                {
                    item.SaleId = sale.Id;
                    _sql.SaveDataInTransaction("dbo.spSaleDetail_Insert", item);
                }
                _sql.CommitTransaction();
            }
            catch
            {
                _sql.RollbackTransaction();
                throw;
            }
        }
        public List<SaleReportModel> GetSaleReport()
        {
            return _sql
                    .LoadData<SaleReportModel, dynamic>(storedProcedure: "dbo.spSale_SaleReport",
                                                        parameters: new { },
                                                        connectionStringName: "RSAData");
        }
    }
}
