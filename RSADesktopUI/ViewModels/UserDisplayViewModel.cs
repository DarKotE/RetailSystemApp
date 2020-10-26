using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using RSADesktopUI.Library.Api;
using RSADesktopUI.Library.Models;

namespace RSADesktopUI.ViewModels
{
    public class UserDisplayViewModel: Screen
    {
        private readonly StatusInfoViewModel _status;
        private readonly IWindowManager _window;
        private readonly IUserEndpoint _userEndpoint;
        private BindingList<ApplicationUserModel> _userList;

        public BindingList<ApplicationUserModel> UserList
        {
            get { return _userList; }
            set
            { 
                _userList = value;
                NotifyOfPropertyChange(() => UserList);
            }
        }

        public UserDisplayViewModel(StatusInfoViewModel status,
                                    IWindowManager window,
                                    IUserEndpoint userEndpoint)
        {
            _status = status;
            _window = window;
            _userEndpoint = userEndpoint;
        }

        protected override async void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            try
            {
                await LoadUsers();
            }
            catch (Exception ex)
            {
                dynamic settings = new ExpandoObject();
                settings.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                settings.ResizeMode = ResizeMode.NoResize;
                settings.Title = "System Error";

                if (ex.Message == "Unauthorized")
                {
                    _status.UpdateMessage("Unauthorized Access", "You have no permission to access Sales Form");
                    _window.ShowDialog(_status, null, settings);
                }
                else
                {
                    _status.UpdateMessage("Fatal exeption", ex.Message);
                    _window.ShowDialog(_status, null, settings);
                }
                TryClose();
            }
        }

        private async Task LoadUsers()
        {
            var users = await _userEndpoint.GetAll();
            UserList = new BindingList<ApplicationUserModel>(users);

        }
    }
}
