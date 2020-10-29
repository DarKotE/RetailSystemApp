using System;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using RSA.DesktopUI.EventModels;
using RSA.DesktopUI.Library.Api;
using RSA.DesktopUI.Library.Models;

namespace RSA.DesktopUI.ViewModels
{
    public class ShellViewModel: Conductor<object>, IHandle<LogOnEventModel>
    {
        private readonly SalesViewModel _salesVM;
        private readonly ILoggedInUserModel _user;
        private readonly IAPIHelper _apiHelper;
        private readonly IEventAggregator _events;


        public ShellViewModel(SalesViewModel salesVM,
                              ILoggedInUserModel user,
                              IAPIHelper apiHelper,
                              IEventAggregator events)
        {
            //start listening to events
            _events = events;
            _events.SubscribeOnPublishedThread(subscriber: this);

            _salesVM = salesVM;
            _user = user;
            _apiHelper = apiHelper;

            //get fresh login instance
            ActivateItemAsync(IoC.Get<LoginViewModel>());
        }

        public bool IsLoggedIn => !String.IsNullOrWhiteSpace(_user.Token);

        public void ExitApp()
        {
            TryCloseAsync();
        }
        public void UserManagement()
        {
            ActivateItemAsync(IoC.Get<UserDisplayViewModel>());
        }


        public async Task LogOut()
        {
            _user.Clear();
            _apiHelper.LogOffUser();
            await ActivateItemAsync(IoC.Get<LoginViewModel>());
            NotifyOfPropertyChange(() => IsLoggedIn);
        }


        public async Task HandleAsync(LogOnEventModel message, CancellationToken cancellationToken)
        {
            //redirect to sales page
            await ActivateItemAsync(_salesVM);
            NotifyOfPropertyChange(() => IsLoggedIn);
        }
    }
}
