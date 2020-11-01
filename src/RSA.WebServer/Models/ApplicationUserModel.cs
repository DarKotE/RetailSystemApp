using System.Collections.Generic;

namespace RSA.WebServer.Models
{
    public class ApplicationUserModel
    {
        public ApplicationUserModel(string id, string email)
        {
            Id = id;
            Email = email;
        }

        public string Id { get; set; }
        public string Email { get; set; }

        //TODO remove roleId from model
        //roleId, roleName 
        public Dictionary<string, string> Roles { get; set; } = new Dictionary<string, string>();
    }
}