using LogViewer.AdminService;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LogViewer.Pages
{
    /// <summary>
    /// Interaction logic for UserPage.xaml
    /// </summary>
    public partial class UserPage : BasePage
    {
        public User User
        {
            get { return (User)GetValue(UserProperty); }
            set { SetValue(UserProperty, value); }
        }

        // Using a DependencyProperty as the backing store for User.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UserProperty =
            DependencyProperty.Register("User", typeof(User), typeof(UserPage), new PropertyMetadata(null));


        public LogEntry[] Log
        {
            get { return (LogEntry[])GetValue(LogProperty); }
            set { SetValue(LogProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Log.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LogProperty =
            DependencyProperty.Register("Log", typeof(LogEntry[]), typeof(UserPage), new PropertyMetadata(new LogEntry[0]));


        public UserPage(string username)
        {
            InitializeComponent();

            Loaded += (a, b) => DoBackgroundWork(BackgroundWork, Completed, "Henter info...", username);
        }

        private void BackgroundWork(DoWorkEventArgs e)
        {
            string username = (string)e.Argument;
            using (var client = GetClient())
            {
                User user = client.GetUser(username) ?? new User { Username = username, Name = "Kunne ikke findes" };

                e.Result = (user, client.GetUserLog(username));
            }
        }

        private void Completed(RunWorkerCompletedEventArgs e)
        {
            if (e.Result != null)
            {
                (User user, LogEntry[] log)? result = e.Result as (User, LogEntry[])?;
                Log = result.Value.log;
                User = result.Value.user;
                Status = $"{Log.Length} entries";
            }
        }

        private void DgLog_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dgLog.SelectedIndex >= 0)
                PageChangeRequest(new ComputerInfoPage((int)dgLog.SelectedValue));
        }
    }
}