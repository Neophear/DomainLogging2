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
    /// Interaction logic for ComputerInfoPage.xaml
    /// </summary>
    public partial class ComputerInfoPage : BasePage
    {
        public LogEntry[] Log
        {
            get { return (LogEntry[])GetValue(LogProperty); }
            set { SetValue(LogProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Log.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LogProperty =
            DependencyProperty.Register("Log", typeof(LogEntry[]), typeof(ComputerInfoPage), new PropertyMetadata(null));

        public ComputerInfo Computer
        {
            get { return (ComputerInfo)GetValue(ComputerProperty); }
            set { SetValue(ComputerProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Computer.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ComputerProperty =
            DependencyProperty.Register("Computer", typeof(ComputerInfo), typeof(ComputerInfoPage), new PropertyMetadata(null));

        public ComputerLocation Location
        {
            get { return (ComputerLocation)GetValue(LocationProperty); }
            set { SetValue(LocationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Location.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LocationProperty =
            DependencyProperty.Register("Location", typeof(ComputerLocation), typeof(ComputerInfoPage), new PropertyMetadata(null));

        public ComputerInfoPage(int computerId)
        {
            InitializeComponent();

            Loaded += (a, b) => DoBackgroundWork(BackgroundWork, Completed, "Henter info...", computerId);
        }

        private void BackgroundWork(DoWorkEventArgs e)
        {
            using (var client = GetClient())
            {
                ComputerInfo computer = client.GetComputerInfoFromId((int)e.Argument);
                ComputerLocation location = client.GetComputerLocation((int)e.Argument);
                LogEntry[] log = client.GetComputerLog((int)e.Argument);
                e.Result = (computer, location, log);
            }
        }

        private void Completed(RunWorkerCompletedEventArgs e)
        {
            (ComputerInfo computer, ComputerLocation location, LogEntry[] log)? result = e.Result as (ComputerInfo, ComputerLocation, LogEntry[])?;

            if (result != null)
            {
                Computer = result.Value.computer;
                Location = result.Value.location;
                Log = result.Value.log;
                Title = Computer.SerialNumber;
                Status = $"{Log.Length} entries";
            }
        }

        private void DgLog_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dgLog.SelectedIndex >= 0)
                PageChangeRequest(new UserPage(((LogEntry)dgLog.SelectedItem).Username));
        }
    }
}