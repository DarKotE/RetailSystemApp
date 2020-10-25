using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RSADataManager.Library.Helpers;
using RSADataManager.Library.Internal.DataAccess;
using RSADataManager.Library.Models;

namespace RSADataManager.Library.DataAccess
{
    public class SaleData
    {
        //TODO remove biz-logic
        public void SaveSale(SaleModel saleInfo, string cashierId)
        {
            var details = new List<SaleDetailDBModel>();
            var products = new ProductData();
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

            var sql = new SqlDataAccess();
            sql.SaveData("dbo.spSale_Insert", sale, "RSAData");
            sale.Id = sql.LoadData<int, dynamic>("spSale_Lookup", new { CashierId = sale.CashierId, SaleDate = sale.SaleDate },"RSAData").FirstOrDefault();
            foreach (var item in details)
            {
                item.SaleId = sale.Id;
                sql.SaveData("dbo.spSaleDetail_Insert", item, "RSAData");
            }
            
        }

    }
}
