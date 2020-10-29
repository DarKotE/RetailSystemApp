using System.Threading.Tasks;
using RSA.DesktopUI.Library.Models;

namespace RSA.DesktopUI.Library.Api
{
    public interface ISaleEndpoint
    {
        Task PostSale(SaleModel sale);
    }
}