using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using RSADesktopUI.EventModels;

namespace RSADesktopUI.ViewModels
{
    public class ShellViewModel: Conductor<object>, IHandle<LogOnEventModel>
    {
        private SalesViewModel _salesVM;
        private IEventAggregator _events;

        public ShellViewModel(SalesViewModel salesVM,
                              IEventAggregator events)
        {
            //start listening to events
            _events = events;
            _events.Subscribe(subscriber: this);

            _salesVM = salesVM;

            //get fresh login instance
            ActivateItem(IoC.Get<LoginViewModel>());
        }

        public void Handle(LogOnEventModel message)
        {
            //redirect to sales page
            ActivateItem(_salesVM);
        }
    }
}
