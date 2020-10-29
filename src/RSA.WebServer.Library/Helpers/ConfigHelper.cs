using System;
using System.Configuration;

namespace RSA.WebServer.Library.Helpers
{
    public class ConfigHelper 
    {
        public static decimal GetTaxRate()
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
