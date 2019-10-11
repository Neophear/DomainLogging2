using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
    /// Interaction logic for AboutPage.xaml
    /// </summary>
    public partial class AboutPage : BasePage
    {
        public IEnumerable<Tuple<string, string, DateTime>> ChangeLog
        {
            get { return (IEnumerable<Tuple<string, string, DateTime>>)GetValue(ChangeLogProperty); }
            set { SetValue(ChangeLogProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ChangeLog.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ChangeLogProperty =
            DependencyProperty.Register("ChangeLog", typeof(IEnumerable<Tuple<string, string, DateTime>>), typeof(AboutPage), new PropertyMetadata(new Tuple<string, string, DateTime>[0]));

        public string AboutText
        {
            get { return (string)GetValue(AboutTextProperty); }
            set { SetValue(AboutTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AboutText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AboutTextProperty =
            DependencyProperty.Register("AboutText", typeof(string), typeof(AboutPage), new PropertyMetadata(""));

        public AboutPage()
        {
            InitializeComponent();

            AboutText = $"Lavet af Stiig Gade\nVersion {Assembly.GetExecutingAssembly().GetName().Version}";

            ChangeLog = new Tuple<string, string, DateTime>[]
            {
                new Tuple<string, string, DateTime>("1.0", "Sweet sweet release!", new DateTime(2019, 8, 1)),
                new Tuple<string, string, DateTime>("1.1", "Tilføjet Om, herunder ChangeLog", new DateTime(2019, 8, 1)),
                new Tuple<string, string, DateTime>("1.1.1", "Fixet visuel bug på Start-siden\nUdvidet manuel versionsstyring (for det har jeg åbenbart brug for!)\nTilføjet dato til ChangeLog", new DateTime(2019, 8, 2)),
                new Tuple<string, string, DateTime>("1.1.2", "Tilføjet funktionalitet til at hente Lokations-info om computere", new DateTime(2019, 8, 21))
            };
        }
    }
}
