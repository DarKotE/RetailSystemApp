using System.Threading.Tasks;
using RSADesktopUI.Library.Models;

namespace RSADesktopUI.Library.Api
{
    public interface ISaleEndpoint
    {
        Task PostSale(SaleModel sale);
    }
}