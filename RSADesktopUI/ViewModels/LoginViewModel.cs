using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;

namespace RSADesktopUI.ViewModels
{
    public class LoginViewModel : Screen
    {
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


        //event fired via cal:Message.Attach.  
        public void OnPasswordChanged(System.Windows.Controls.PasswordBox source)
        {
            Password = source.Password;
        }


        public bool CanLogIn => 
            UserName?.Length > 0 && Password?.Length > 0;


        public void LogIn()
        {
            throw new NotImplementedException();
        } 

    }
}
