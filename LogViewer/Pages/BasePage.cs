using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace LogViewer.Pages
{
    public abstract class BasePage : Page
    {
        public List<MenuItem> Menu { get; } = new List<MenuItem>();

        public delegate void PageChangeRequestEventHandler(object sender, BasePage newPage);

        public event PageChangeRequestEventHandler OnPageChangeRequest;

        public bool IsBusy
        {
            get { return (bool)GetValue(IsBusyProperty); }
            set { SetValue(IsBusyProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsBusy.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsBusyProperty =
            DependencyProperty.Register("IsBusy", typeof(bool), typeof(BasePage), new PropertyMetadata(false));


        public string BusyMessage
        {
            get { return (string)GetValue(BusyMessageProperty); }
            set { SetValue(BusyMessageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BusyMessage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BusyMessageProperty =
            DependencyProperty.Register("BusyMessage", typeof(string), typeof(BasePage), new PropertyMetadata(""));



        #region Status
        public string Status
        {
            get { return (string)GetValue(StatusProperty); }
            set { SetValue(StatusProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Status.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StatusProperty =
            DependencyProperty.Register("Status", typeof(string), typeof(BasePage), new PropertyMetadata(null));
        #endregion

        protected AdminService.AdminServiceClient GetClient()
        {
            return new AdminService.AdminServiceClient(new BasicHttpBinding
            {
                MaxReceivedMessageSize = 2147483647,
                Security = new BasicHttpSecurity
                {
                    Mode = BasicHttpSecurityMode.TransportCredentialOnly,
                    Transport = new HttpTransportSecurity { ClientCredentialType = HttpClientCredentialType.Windows }
                }
            },
            //new EndpointAddress("http://localhost:2870/AdminService.svc"));
            new EndpointAddress("http://dlservice:6970/AdminService.svc"));
        }

        protected void DoBackgroundWork(Action<DoWorkEventArgs> work, Action<RunWorkerCompletedEventArgs> onCompleted, string busyMessage, object argument = null)
        {
            using (var worker = new BackgroundWorker())
            {
                BusyMessage = busyMessage;
                IsBusy = true;

                worker.DoWork += (a, b) => work(b);
                worker.RunWorkerCompleted += (a, b) =>
                {
                    IsBusy = false;

                    if (b.Error == null)
                        onCompleted(b);
                    else
                        PageChangeRequest(new ErrorPage(b.Error));
                };

                worker.RunWorkerAsync(argument);
            }
        }

        protected void PageChangeRequest(BasePage newPage)
        {
            OnPageChangeRequest?.Invoke(this, newPage);
        }

        protected void AddMenuItem(string header, Action clickEvent)
        {
            MenuItem menuItem = new MenuItem { Header = header };
            menuItem.Click += (_, __) => clickEvent();
            Menu.Add(menuItem);
        }
    }
}
