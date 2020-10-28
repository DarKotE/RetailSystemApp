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
    public class UserDisplayViewModel : Screen
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
        private ApplicationUserModel _selectedUser;
        public ApplicationUserModel SelectedUser
        {
            get
            {
                return _selectedUser;
            }
            set
            {
                _selectedUser = value;
                SelectedUserName = value.Email;
                UserRoles.Clear();
                UserRoles = new BindingList<string>(value.Roles.Select(x => x.Value).ToList());
                AvailableRoles = new BindingList<string>(PreloadedAvailableRoles.Except(UserRoles).ToList());
                NotifyOfPropertyChange(() => AvailableRoles);
                NotifyOfPropertyChange(() => SelectedUser);
            }
        }

        private string _selectedUserName;
        public string SelectedUserName
        {
            get { return _selectedUserName; }
            set 
            { 
                _selectedUserName = value;
                NotifyOfPropertyChange(() => SelectedUserName);
            }
        }

        private BindingList<string> _userRoles = new BindingList<string>();
        public BindingList<string> UserRoles
        {
            get { return _userRoles; }
            set 
            { 
                _userRoles = value;
                NotifyOfPropertyChange(() => SelectedUser.RoleList);
                NotifyOfPropertyChange(() => UserRoles);
            }
        }

        private BindingList<string> _availableRoles = new BindingList<string>();
        public BindingList<string> AvailableRoles
        {
            get { return _availableRoles; }
            set
            {
                _availableRoles = value;
                NotifyOfPropertyChange(() => AvailableRoles);
            }
        }

        public BindingList<string> PreloadedAvailableRoles { get; set; }

        private string _selectedUserRole;
        public string SelectedUserRole
        {
            get { return _selectedUserRole; }
            set 
            { 
                _selectedUserRole = value;
                NotifyOfPropertyChange(() => SelectedUserRole);
            }
        }

        private string _selectedAvailableRole;
        public string SelectedAvailableRole
        {
            get { return _selectedAvailableRole; }
            set
            {
                _selectedAvailableRole = value;
                NotifyOfPropertyChange(() => SelectedAvailableRole);
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
                await LoadRoles();
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

        private async Task LoadRoles()
        {
            var roles = await _userEndpoint.GetAllRoles();
            PreloadedAvailableRoles = new BindingList<string>(roles.Select(x => x.Value).ToList());
        }

        public async Task AddSelectedRole()
        {
            await _userEndpoint.AddUserToRole(SelectedUser.Id, SelectedAvailableRole);
            UserRoles.Add(SelectedAvailableRole);
            AvailableRoles.Remove(SelectedAvailableRole);
        }
        public async Task RemoveSelectedRole()
        {
            await _userEndpoint.RemoveUserFromRole(SelectedUser.Id, SelectedUserRole);
            AvailableRoles.Add(SelectedUserRole);
            UserRoles.Remove(SelectedUserRole);
        }
    }
}
