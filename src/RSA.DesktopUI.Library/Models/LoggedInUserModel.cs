using System;

namespace RSA.DesktopUI.Library.Models
{
    public class LoggedInUserModel : ILoggedInUserModel
    {
        public string Token { get; set; }
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public DateTime CreatedDate { get; set; }

        public void Clear()
        {
            Token = String.Empty;
            Id = String.Empty;
            FirstName = String.Empty;
            LastName = String.Empty;
            EmailAddress = String.Empty;
            CreatedDate = DateTime.MinValue;
        }
    }
}
