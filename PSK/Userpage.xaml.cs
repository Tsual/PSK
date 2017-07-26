using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace PSK
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class Userpage : Page
    {
        public ObservableCollection<Models.Info> ItemCollection = Core.Current.CurrentUser.Recordings;
        public Userpage()
        {
            this.InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Core.Current.Unsubscribe();
            if (Frame.CanGoBack)
                Frame.GoBack();
        }

        private void _StackPanel_Loaded(object sender, RoutedEventArgs e)
        {
            var _list = Core.Current.CurrentUser.Recordings;
            if(_list.Count==0)
            {
                TextBlock tb = new TextBlock() { Text = "没有记录", HorizontalAlignment = HorizontalAlignment.Center };
                _StackPanel.Children.Add(tb);
            }
            else
            {
                for(int i=0;i<_list.Count;i++)
                {
                    var obj = _list[i];
                    DetailCtrl ctrl = new DetailCtrl() { InfoItem = obj, ItemIndex = i+1 };
                    _StackPanel.Children.Add(ctrl);
                }
            }
            Button_Click_1(null, null);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            foreach (var t in _StackPanel.Children.ToList())
            {
                ((DetailCtrl)t).ChangeState();
            }
        }
    }
}
