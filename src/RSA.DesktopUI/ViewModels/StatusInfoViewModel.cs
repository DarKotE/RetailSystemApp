using System;
using Caliburn.Micro;

namespace RSA.DesktopUI.ViewModels
{
    public class StatusInfoViewModel: Screen
    {
        public string Header { get; private set; } = String.Empty;
        public string Message { get; private set; } = String.Empty;

        public void UpdateMessage(string header, string message)
        {
            Header = header;
            Message = message;

            NotifyOfPropertyChange(() => Header);
            NotifyOfPropertyChange(() => Message);
        }
        public void Close()
        {
            TryCloseAsync();
        }
    }
}
