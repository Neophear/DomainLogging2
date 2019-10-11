using System;
using System.Reflection;
using System.Windows;

namespace LogViewer.Pages
{
    /// <summary>
    /// Interaction logic for HomePage.xaml
    /// </summary>
    public partial class HomePage : BasePage
    {
        public HomePage()
        {
            InitializeComponent();

            Status = $"Made by Stiig Gade - Version {Assembly.GetExecutingAssembly().GetName().Version}";
        }

        private void ShowLog(object sender, RoutedEventArgs e)
        {
            PageChangeRequest(new LogPage());
        }

        private void ShowComputers(object sender, RoutedEventArgs e)
        {
            PageChangeRequest(new ComputersPage());
        }

        private void FindUser(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(wtxtUsername.Text))
                PageChangeRequest(new UserPage(wtxtUsername.Text));
        }
    }
}
