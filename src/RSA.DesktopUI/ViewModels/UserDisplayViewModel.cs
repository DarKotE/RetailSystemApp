using System;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
using Caliburn.Micro;
using RSA.DesktopUI.Library.Api;
using RSA.DesktopUI.Library.Models;

namespace RSA.DesktopUI.ViewModels
{
    public class UserDisplayViewModel : Screen
    {
        private readonly StatusInfoViewModel _status;
        private readonly IWindowManager _window;
        private readonly IUserEndpoint _userEndpoint;

        private BindingList<ApplicationUserModel>? _userList;
        public BindingList<ApplicationUserModel>? UserList
        {
            get => _userList;
            set
            {
                _userList = value;
                NotifyOfPropertyChange(() => UserList);
            }
        }
        private ApplicationUserModel? _selectedUser;
        public ApplicationUserModel? SelectedUser
        {
            get => _selectedUser;
            set
            {
                if (value != null)
                {
                    _selectedUser = value;
                    SelectedUserName = value.Email;
                    UserRoles = new BindingList<string>(value.Roles.Select(x => x.Value).ToList());
                    AvailableRoles = new BindingList<string>(PreloadedAvailableRoles.Except(UserRoles).ToList());
                }
                NotifyOfPropertyChange(() => AvailableRoles);
                NotifyOfPropertyChange(() => SelectedUser);
            }
        }

        private string _selectedUserName = String.Empty;
        public string SelectedUserName
        {
            get => _selectedUserName;
            set
            {
                _selectedUserName = value;
                NotifyOfPropertyChange(() => SelectedUserName);
            }
        }
            //= new BindingList<string>()
        private BindingList<string>? _userRoles;
        public BindingList<string>? UserRoles
        {
            get => _userRoles;
            set
            {
                _userRoles = value;
                NotifyOfPropertyChange(() => UserRoles);
            }
        }

        private BindingList<string>? _availableRoles;
        public BindingList<string>? AvailableRoles
        {
            get => _availableRoles;
            set
            {
                _availableRoles = value;
                NotifyOfPropertyChange(() => AvailableRoles);
            }
        }

        public BindingList<string>? PreloadedAvailableRoles { get; private set; }

        private string? _selectedUserRole;
        public string? SelectedUserRole
        {
            get => _selectedUserRole;
            set
            {
                _selectedUserRole = value;
                NotifyOfPropertyChange(() => SelectedUserRole);
                NotifyOfPropertyChange(() => SelectedAvailableRole);
                NotifyOfPropertyChange(() => CanRemoveSelectedRole);
                NotifyOfPropertyChange(() => CanAddSelectedRole);
            }
        }

        private string? _selectedAvailableRole;
        public string? SelectedAvailableRole
        {
            get => _selectedAvailableRole;
            set
            {
                _selectedAvailableRole = value;
                NotifyOfPropertyChange(() => CanAddSelectedRole);
                NotifyOfPropertyChange(() => CanRemoveSelectedRole);
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
                    await _window.ShowDialogAsync(_status, null, settings);
                }
                else
                {
                    _status.UpdateMessage("Fatal exception", ex.Message);
                    await _window.ShowDialogAsync(_status, null, settings);
                }
                await TryCloseAsync();
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

        public bool CanAddSelectedRole => SelectedAvailableRole != null;

        public bool CanRemoveSelectedRole => SelectedUserRole != null;


        public async Task AddSelectedRole()
        {
            if (SelectedUser != null && SelectedAvailableRole != null && UserRoles != null && AvailableRoles != null)
            {
                await _userEndpoint.AddUserToRole(SelectedUser.Id, SelectedAvailableRole);
                UserRoles.Add(SelectedAvailableRole);
                AvailableRoles.Remove(SelectedAvailableRole);
                NotifyOfPropertyChange(() => CanAddSelectedRole);
                NotifyOfPropertyChange(() => CanRemoveSelectedRole);
            }
            else
            {
                // Should be unreachable since button should be disabled
                // TODO simplify
                dynamic settings = new ExpandoObject();
                settings.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                settings.ResizeMode = ResizeMode.NoResize;
                settings.Title = "Null reference exception";
                {
                    _status.UpdateMessage("Fatal exception", "Null reference exception");
                    await _window.ShowDialogAsync(_status, null, settings);
                }
                await TryCloseAsync();
            }
        }
        public async Task RemoveSelectedRole()
        {
            if (SelectedUser != null && SelectedUserRole != null && UserRoles != null && AvailableRoles != null)
            {
                await _userEndpoint.RemoveUserFromRole(SelectedUser.Id, SelectedUserRole);
                AvailableRoles.Add(SelectedUserRole);
                UserRoles.Remove(SelectedUserRole);
                NotifyOfPropertyChange(() => CanAddSelectedRole);
                NotifyOfPropertyChange(() => CanRemoveSelectedRole);
            }
            else
            {
                // Should be unreachable since button should be disabled
                // TODO simplify
                dynamic settings = new ExpandoObject();
                settings.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                settings.ResizeMode = ResizeMode.NoResize;
                settings.Title = "Null reference exception";
                {
                    _status.UpdateMessage("Fatal exception", "Null reference exception");
                    await _window.ShowDialogAsync(_status, null, settings);
                }
                await TryCloseAsync();
            }
        }
    }
}

