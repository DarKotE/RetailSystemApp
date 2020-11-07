using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using RSA.DesktopUI.Library.Models;

namespace RSA.DesktopUI.Library.Api
{
    public class ApiHelper : IApiHelper
    {
        private HttpClient _apiClient;
        private readonly ILoggedInUserModel _loggedInUser;
        private readonly IConfiguration _config;

        public ApiHelper(ILoggedInUserModel loggedInUser, IConfiguration config)
        {
            _loggedInUser = loggedInUser;
            _config = config;
            InitializeClient();
        }

        public HttpClient ApiClient
        {
            get
            {
                return _apiClient;
            }
        }

        private void InitializeClient()
        {
            _apiClient = new HttpClient();

            string apiPath = _config.GetValue<string>("api");
            _apiClient.BaseAddress = new Uri(uriString: apiPath);

            _apiClient.DefaultRequestHeaders.Accept.Clear();
            _apiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        }

        public async Task<AuthenticatedUser> Authenticate(string username, string password)
        {
            using var data = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "password"),
                new KeyValuePair<string, string>("username", username),
                new KeyValuePair<string, string>("password", password),
            });
            using HttpResponseMessage response = await _apiClient.PostAsync("/Token", data);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<AuthenticatedUser>();
            }
            else
            {
                throw new Exception(response.ReasonPhrase);
            }
        }

        public void LogOffUser()
        {
            _apiClient.DefaultRequestHeaders.Clear();
        }

        public async Task GetLoggedInUserInfo(string token)
        {
            _apiClient.DefaultRequestHeaders.Clear();
            _apiClient.DefaultRequestHeaders.Accept.Clear();
            _apiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _apiClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            using HttpResponseMessage response = await _apiClient.GetAsync("/api/User");
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsAsync<LoggedInUserModel>();
                _loggedInUser.Token = token;
                _loggedInUser.Id = result.Id;
                _loggedInUser.FirstName = result.FirstName;
                _loggedInUser.LastName = result.LastName;
                _loggedInUser.CreatedDate = result.CreatedDate;
                _loggedInUser.EmailAddress = result.EmailAddress;
            }
            else
            {
                throw new Exception(response.ReasonPhrase);
            }
        }
    }
}
