using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using RSA.DesktopUI.Library.Models;

namespace RSA.DesktopUI.Library.Api
{
    public class ProductEndpoint : IProductEndpoint
    {
        private readonly IApiHelper _apiHelper;
        public ProductEndpoint(IApiHelper apiHelper)
        {
            _apiHelper = apiHelper;
        }

        public async Task<List<ProductModel>> GetAll()
        {
            using HttpResponseMessage response = await _apiHelper.ApiClient.GetAsync("/api/Product");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<List<ProductModel>>();
            }
            else
            {
                throw new Exception(response.ReasonPhrase);
            }
        }
    }
}
