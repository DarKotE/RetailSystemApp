using System;
using System.Configuration;

namespace RSA.DesktopUI.Library.Helpers
{
    public class ConfigHelper : IConfigHelper
    {
        public decimal GetTaxRate()
        {
            string rateText = ConfigurationManager.AppSettings["taxRate"];

            bool isTaxValid = Decimal.TryParse(rateText,
                                              result: out decimal output);

            if (isTaxValid)
            {
                return output;
            }

            throw new ConfigurationErrorsException("The tax rate is not set up properly");
        }
    }
}
