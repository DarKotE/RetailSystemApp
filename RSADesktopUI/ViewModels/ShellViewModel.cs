using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using RSADesktopUI.EventModels;
using RSADesktopUI.Library.Models;

namespace RSADesktopUI.ViewModels
{
    public class ShellViewModel: Conductor<object>, IHandle<LogOnEventModel>
    {
        private SalesViewModel _salesVM;
        private ILoggedInUserModel _user;
        private IEventAggregator _events;

        public ShellViewModel(SalesViewModel salesVM,
                              ILoggedInUserModel user,
                              IEventAggregator events)
        {
            //start listening to events
            _events = events;
            _events.Subscribe(subscriber: this);

            _salesVM = salesVM;
            _user = user;

            //get fresh login instance
            ActivateItem(IoC.Get<LoginViewModel>());
        }

        public bool IsLoggedIn => !String.IsNullOrWhiteSpace(_user.Token);

        public void ExitApp()
        {
            TryClose();
        }

        public void LogOut()
        {
            _user.Clear();
            ActivateItem(IoC.Get<LoginViewModel>());
            NotifyOfPropertyChange(() => IsLoggedIn);
        }

        public void Handle(LogOnEventModel message)
        {
            //redirect to sales page
            ActivateItem(_salesVM);
            NotifyOfPropertyChange(() => IsLoggedIn);
        }
    }
}
