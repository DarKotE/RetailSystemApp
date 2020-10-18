using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using RSADesktopUI.Helpers;

namespace RSADesktopUI.ViewModels
{
    public class LoginViewModel : Screen
    {
        private IAPIHelper _apiHelper;
        public LoginViewModel(IAPIHelper apiHelper)
        {
            _apiHelper = apiHelper;
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

        //WPF forbids passwordbox binding, so it is done via Caliburn.Micro Action 
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
                await _apiHelper.Authenticate(UserName, Password);
                ErrorMessage = String.Empty;
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            
            }
        } 

    }
}
