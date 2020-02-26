using Microsoft.Toolkit.Win32.UI.Controls.Interop.WinRT;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace Wonton.WinUI.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            webview.NavigateToLocalStreamUri(new Uri(System.IO.Path.Combine("index.html"), UriKind.Relative), new MyResolver());
        }
    }

    class MyResolver : IUriToStreamResolver
    {
        public Stream UriToStream(Uri uri)
        {
            var cur = Environment.CurrentDirectory;
            var rel = uri.LocalPath;
            var abs = cur + "/build" + rel;
            return File.Open(abs, FileMode.Open);
        }
    }
}
