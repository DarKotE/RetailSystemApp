using System;
using System.Threading.Tasks;
using Caliburn.Micro;
using RSA.DesktopUI.EventModels;
using RSA.DesktopUI.Library.Api;

namespace RSA.DesktopUI.ViewModels
{
    public class LoginViewModel : Screen
    {
        private readonly IApiHelper _apiHelper;
        private readonly IEventAggregator _events;

        public LoginViewModel(IApiHelper apiHelper, IEventAggregator events)
        {
            _apiHelper = apiHelper;
            _events = events;
        }

        private string _userName = "evgenlight@yandex.ru";

        public string UserName
        {
            get => _userName;
            set
            {
                _userName = value;
                NotifyOfPropertyChange(() => UserName);
            }
        }

        //WPF forbids direct passwordbox binding, so it is done via Caliburn.Micro Action
        private string _password = "!QAZ2wsx";

        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                NotifyOfPropertyChange(() => Password);
                NotifyOfPropertyChange(() => CanLogIn);
            }
        }

        public bool IsErrorVisible => !String.IsNullOrWhiteSpace(ErrorMessage);

        private string? _errorMessage;

        public string? ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
                NotifyOfPropertyChange(() => IsErrorVisible);
                NotifyOfPropertyChange(() => ErrorMessage);
            }
        }

        //event fired via cal:Message.Attach.
        public void OnPasswordChanged(System.Windows.Controls.PasswordBox source)
        {
            Password = source.Password;
        }

        public bool CanLogIn =>
            //condition
            UserName?.Length > 0
            && Password?.Length > 0;

        public async Task LogIn()
        {
            try
            {
                var result = await _apiHelper.Authenticate(UserName, Password);
                //fetch additional information on user
                await _apiHelper.GetLoggedInUserInfo(result.Access_Token);

                //in absence of exception - clear error message text block and hide it
                ErrorMessage = String.Empty;

                await _events.PublishOnUIThreadAsync(new LogOnEventModel());
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
        }
    }
}