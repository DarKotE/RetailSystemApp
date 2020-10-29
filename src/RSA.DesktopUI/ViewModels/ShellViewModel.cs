using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using RSADesktopUI.EventModels;
using RSADesktopUI.Library.Api;
using RSADesktopUI.Library.Models;

namespace RSADesktopUI.ViewModels
{
    public class ShellViewModel: Conductor<object>, IHandle<LogOnEventModel>
    {
        private SalesViewModel _salesVM;
        private ILoggedInUserModel _user;
        private IAPIHelper _apiHelper;
        private IEventAggregator _events;


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

        public async Task ExitApp()
        {
            TryCloseAsync();
        }
        public async Task UserManagement()
        {
            await ActivateItemAsync(IoC.Get<UserDisplayViewModel>());

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
