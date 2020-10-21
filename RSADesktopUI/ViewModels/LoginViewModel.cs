using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using RSADesktopUI.EventModels;
using RSADesktopUI.Library.Api;

namespace RSADesktopUI.ViewModels
{
    public class LoginViewModel : Screen
    {
        private IAPIHelper _apiHelper;
        private IEventAggregator _events;

        public LoginViewModel(IAPIHelper apiHelper, IEventAggregator events)
        {
            _apiHelper = apiHelper;
            _events = events;
        }
        

        private string _userName;
        public string UserName
        {
            get { return _userName; }
            set
            {
                _userName = value;
                NotifyOfPropertyChange(() => UserName);
            }
        }

        //WPF forbids direct passwordbox binding, so it is done via Caliburn.Micro Action
        private string _password;
        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                NotifyOfPropertyChange(() => Password);
                NotifyOfPropertyChange(() => CanLogIn);
            }
        }
        public bool IsErrorVisible => !String.IsNullOrWhiteSpace(ErrorMessage);

        private string _errorMessage;
        public string ErrorMessage
        {
            get { return _errorMessage; }
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
            UserName?.Length > 0 && Password?.Length > 0;


        public async Task LogIn()
        {
            try
            {
                var result = await _apiHelper.Authenticate(UserName, Password);
                //fetch additional information on user
                await _apiHelper.GetLoggedInUserInfo(result.Access_Token);

                //in absence of exeption clear errormessage textblock errors and hide it
                ErrorMessage = String.Empty;

                _events.PublishOnUIThread(new LogOnEventModel());
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            
            }
        } 

    }
}
