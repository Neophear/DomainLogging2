using LogViewer.AdminService;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace LogViewer.Pages
{
    /// <summary>
    /// Interaction logic for LogPage.xaml
    /// </summary>
    public partial class LogPage : BasePage
    {
        public LogEntry[] Log
        {
            get { return (LogEntry[])GetValue(LogProperty); }
            set { SetValue(LogProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Log.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LogProperty =
            DependencyProperty.Register("Log", typeof(LogEntry[]), typeof(LogPage), new PropertyMetadata(new LogEntry[0]));

        public LogPage()
        {
            InitializeComponent();

            AddMenuItem("Opdater", LoadLog);

            Loaded += (a, b) => LoadLog();
        }

        private void LoadLog()
        {
            DoBackgroundWork(BackgroundWork, BackgroundWorkComplete, "Henter log...");
        }

        private void BackgroundWork(DoWorkEventArgs e)
        {
            using (var client = GetClient())
                e.Result = client.GetLog(500);
        }

        private void BackgroundWorkComplete(RunWorkerCompletedEventArgs e)
        {
            Log = e.Result as LogEntry[];
            Status = $"{Log.Length} entries (max 500)";
        }

        private void DgLog_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dgLog.SelectedIndex >= 0)
                PageChangeRequest(new ComputerInfoPage((int)dgLog.SelectedValue));
        }

        private void GoToUser(object sender, RoutedEventArgs e)
        {
            if (dgLog.SelectedIndex >= 0)
                PageChangeRequest(new UserPage(((LogEntry)dgLog.SelectedItem).Username));
        }

        private void GoToComputer(object sender, RoutedEventArgs e)
        {
            if (dgLog.SelectedIndex >= 0)
                PageChangeRequest(new ComputerInfoPage(((LogEntry)dgLog.SelectedItem).ComputerId));
        }
    }
}