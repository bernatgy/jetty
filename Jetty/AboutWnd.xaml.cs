using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Shapes;

namespace Jetty
{
    /// <summary>
    /// Interaction logic for AboutWnd.xaml
    /// </summary>
    public partial class AboutWnd : Window
    {
        public AboutWnd()
        {
            InitializeComponent();

            var ass = Assembly.GetExecutingAssembly();
            var fvi = FileVersionInfo.GetVersionInfo(ass.Location);

            this.appNameLabel.Content = fvi.ProductName;
            this.appDescLabel.Content = fvi.Comments;
            this.appVerLabel.Content = fvi.FileVersion;
            this.appLegalLabel.Content = fvi.LegalCopyright;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
