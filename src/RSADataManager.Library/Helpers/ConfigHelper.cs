using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSADataManager.Library.Helpers
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
