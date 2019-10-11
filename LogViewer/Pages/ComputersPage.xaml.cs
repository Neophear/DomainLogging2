using LogViewer.AdminService;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace LogViewer.Pages
{
    /// <summary>
    /// Interaction logic for ComputersPage.xaml
    /// </summary>
    public partial class ComputersPage : BasePage
    {
        public ComputerInfo[] Computers
        {
            get { return (ComputerInfo[])GetValue(ComputersProperty); }
            set { SetValue(ComputersProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Computers.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ComputersProperty =
            DependencyProperty.Register("Computers", typeof(ComputerInfo[]), typeof(ComputersPage), new PropertyMetadata(new ComputerInfo[0]));

        public ComputersPage()
        {
            InitializeComponent();

            AddMenuItem("Eksporter CSV", ExportCSV);

            Loaded += (a, b) => DoBackgroundWork(BackgroundWork, Completed, "Henter computere...");
        }

        private void BackgroundWork(DoWorkEventArgs e)
        {
            using (var client = GetClient())
                e.Result = client.GetComputerInfos();
        }

        private void Completed(RunWorkerCompletedEventArgs e)
        {
            if (e.Result is ComputerInfo[] result)
                Computers = result;

            Status = $"{Computers.Length} computere";
        }

        private void CollectionViewSource_Filter(object sender, FilterEventArgs e)
        {
            string filter = wtxtFilter.Text.ToUpper();

            if (String.IsNullOrEmpty(filter))
                e.Accepted = true;
            else
                e.Accepted = e.Item is ComputerInfo c ? c.SerialNumber.Contains(filter) || c.Name.Contains(filter) : false;
        }

        private void WtxtFilter_KeyUp(object sender, KeyEventArgs e)
        {
            ICollectionView view = CollectionViewSource.GetDefaultView(dgComputers.ItemsSource);
            view.Refresh();
            Status = $"{view.Cast<ComputerInfo>().Count()} computere";
        }

        private void ExportCSV()
        {
            SaveFileDialog dialog = new SaveFileDialog
            {
                FileName = "Computere.csv",
                AddExtension = true,
                DefaultExt = ".csv",
                Filter = "Semikolonsepareret fil (*.csv)|*.csv|Alle Filer (*.*)|*.*"
            };

            if (dialog.ShowDialog() ?? false)
            {
                IEnumerable<ComputerInfo> computers = CollectionViewSource.GetDefaultView(dgComputers.ItemsSource).Cast<ComputerInfo>();
                List<string> output = new List<string>(computers.Count() + 1);

                output.Add("Serienummer;Navn;Model;OS;CPU;CPU kerner;RAM;Disk type;Disk størrelse;GFX;TeamViewer");
                output.AddRange(computers.Select(ci => $"{ci.SerialNumber};{ci.Name};{ci.Model};{ci.OS};{ci.CPU};{ci.CPUCores};{ci.RAMGB};{ci.DiskMediaType};{ci.DiskSize};{ci.GFX};{ci.TeamViewerId}"));
                
                try
                {
                    File.WriteAllLines(dialog.FileName, output, Encoding.UTF8);
                    System.Diagnostics.Process.Start(Path.GetDirectoryName(dialog.FileName));
                }
                catch (Exception e)
                {
                    PageChangeRequest(new ErrorPage(e));
                }
            }
        }

        private void DgComputers_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dgComputers.SelectedIndex >= 0)
                PageChangeRequest(new ComputerInfoPage((int)dgComputers.SelectedValue));
        }
    }
}