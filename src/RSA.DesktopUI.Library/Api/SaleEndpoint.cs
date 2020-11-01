using System;
using System.Net.Http;
using System.Threading.Tasks;
using RSA.DesktopUI.Library.Models;

namespace RSA.DesktopUI.Library.Api
{
    public class SaleEndpoint : ISaleEndpoint
    {
        private readonly IApiHelper _apiHelper;
        public SaleEndpoint(IApiHelper apiHelper)
        {
            _apiHelper = apiHelper;
        }

        public async Task PostSale(SaleModel sale)
        {
            using HttpResponseMessage response = await _apiHelper.ApiClient.PostAsJsonAsync("/api/Sale", sale);
            if (response.IsSuccessStatusCode)
            {
                // TODO What to do after sale
            }
            else
            {
                throw new Exception(response.ReasonPhrase);
            }
        }
    }
}
