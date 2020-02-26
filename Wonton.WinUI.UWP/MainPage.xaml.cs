using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Web;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace Wonton.WinUI.UWP
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            webview.NavigateToLocalStreamUri(new Uri(System.IO.Path.Combine("index.html"), UriKind.Relative), new MyResolver());
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

            public IAsyncOperation<IInputStream> UriToStreamAsync(Uri uri)
            {
                throw new NotImplementedException();
            }
        }
    }
}
