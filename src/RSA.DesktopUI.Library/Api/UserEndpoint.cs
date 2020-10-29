using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using RSA.DesktopUI.Library.Models;

namespace RSA.DesktopUI.Library.Api
{
    public class UserEndpoint : IUserEndpoint
    {
        private readonly IApiHelper _apiHelper;

        public UserEndpoint(IApiHelper apiHelper)
        {
            _apiHelper = apiHelper;
        }

        public async Task<List<ApplicationUserModel>> GetAll()
        {
            using HttpResponseMessage response = await _apiHelper.ApiClient.GetAsync("/api/User/Admin/GetAllUsers");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<List<ApplicationUserModel>>();
            }
            else
            {
                throw new Exception(response.ReasonPhrase);
            }
        }

        public async Task<Dictionary<string, string>> GetAllRoles()
        {
            using HttpResponseMessage response = await _apiHelper.ApiClient.GetAsync("/api/User/Admin/GetAllRoles");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<Dictionary<string, string>>();
            }
            else
            {
                throw new Exception(response.ReasonPhrase);
            }
        }

        public async Task AddUserToRole(string userId, string roleName)
        {
            var data = new { userId, roleName };

            using HttpResponseMessage response = await _apiHelper.ApiClient.PostAsJsonAsync("/api/User/Admin/AddRole", data);
            if (response.IsSuccessStatusCode)
            {
                // TODO logging?
            }
            else
            {
                throw new Exception(response.ReasonPhrase);
            }
        }

        public async Task RemoveUserFromRole(string userId, string roleName)
        {
            var data = new { userId, roleName };

            using HttpResponseMessage response = await _apiHelper.ApiClient.PostAsJsonAsync("/api/User/Admin/RemoveRole", data);
            if (response.IsSuccessStatusCode)
            {
                // TODO logging?
            }
            else
            {
                throw new Exception(response.ReasonPhrase);
            }
        }
    }
}
