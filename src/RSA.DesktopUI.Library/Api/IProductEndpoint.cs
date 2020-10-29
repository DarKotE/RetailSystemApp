using System.Collections.Generic;
using System.Threading.Tasks;
using RSA.DesktopUI.Library.Models;

namespace RSA.DesktopUI.Library.Api
{
    public interface IProductEndpoint
    {
        Task<List<ProductModel>> GetAll();
    }
}