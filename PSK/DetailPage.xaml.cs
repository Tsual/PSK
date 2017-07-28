using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
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
    public sealed partial class DetailPage : Page
    {
        public DetailPage()
        {
            this.InitializeComponent();
            if (Core.Current.DetailPage_databridge == null)
            {
                if (Frame.CanGoBack)
                    Frame.GoBack();
                else
                    throw new NullReferenceException();
            }
            else
            {
                _UI_Info = Core.Current.DetailPage_databridge;
                Core.Current.DetailPage_databridge = null;
            }
            
        }



        public Models.UI_Info _UI_Info { get; set; }
        public string _Title
        {
            get
            {
                return Core.Current.CurrentUser.PID + "/" + _UI_Info._Info.DetailName;
            }
        }

        public ObservableCollection<Models.UI_Info_Str> Items { get { return _UI_Info.Lines; } }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (c_flyout_tb.Text == "")
                return;
            Items.Add(new Models.UI_Info_Str() { str = c_flyout_tb.Text });
            c_flyout_tb.Text = "";
            c_add_flyout.Hide();
        }
    }
}
