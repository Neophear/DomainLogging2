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

namespace LogViewer.Pages
{
    /// <summary>
    /// Interaction logic for ErrorPage.xaml
    /// </summary>
    public partial class ErrorPage : BasePage
    {
        public Visibility MessageVisibility
        {
            get { return String.IsNullOrEmpty(ErrorMessage) ? Visibility.Hidden : Visibility.Visible; }
        }

        public string ErrorTitle
        {
            get { return (string)GetValue(ErrorTitleProperty); }
            set { SetValue(ErrorTitleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ErrorTitle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ErrorTitleProperty =
            DependencyProperty.Register("ErrorTitle", typeof(string), typeof(ErrorPage), new PropertyMetadata("Der skete en uventet fejl!"));

        public string ErrorMessage
        {
            get { return (string)GetValue(ErrorMessageProperty); }
            set { SetValue(ErrorMessageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ErrorMessage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ErrorMessageProperty =
            DependencyProperty.Register("ErrorMessage", typeof(string), typeof(ErrorPage), new PropertyMetadata(""));

        public ErrorPage(Exception e)
        {
            InitializeComponent();

            if (e is System.ServiceModel.EndpointNotFoundException)
                ErrorTitle = "Kunne ikke forbinde til servicen!";
            else
            {
                ErrorTitle = "Der skete en uventet fejl!";
                ErrorMessage = e.Message;
            }
        }
    }
}
