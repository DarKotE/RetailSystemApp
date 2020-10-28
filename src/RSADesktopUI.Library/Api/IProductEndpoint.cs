using System.Collections.Generic;
using System.Threading.Tasks;
using RSADesktopUI.Library.Models;

namespace RSADesktopUI.Library.Api
{
    public interface IProductEndpoint
    {
        Task<List<ProductModel>> GetAll();
    }
}