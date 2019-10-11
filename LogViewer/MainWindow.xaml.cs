using LogViewer.AdminService;
using System;
using System.Collections.Generic;
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

namespace LogViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            ChangePage(new Pages.HomePage());
        }

        public void ChangePage(Pages.BasePage p)
        {
            if (frmContent.Content != null)
                foreach (var menuItem in ((Pages.BasePage)frmContent.Content).Menu)
                    mnuMain.Items.Remove(menuItem);

            p.OnPageChangeRequest += (s, newPage) => ChangePage(newPage);

            frmContent.Navigate(p);

            foreach (var menuItem in p.Menu)
                mnuMain.Items.Add(menuItem);
        }

        private void MenuFileAbout(object sender, RoutedEventArgs e)
        {
            ChangePage(new Pages.AboutPage());
        }

        private void MenuFileExit(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void MenuStartPage(object sender, RoutedEventArgs e)
        {
            ChangePage(new Pages.HomePage());
        }
    }
}