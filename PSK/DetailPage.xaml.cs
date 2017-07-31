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



        #region Delete button funcs
        Models.UI_Info_Str _del_item = null;
        private void ListView_DragItemsStarting(object sender, DragItemsStartingEventArgs e)
        {
            _del_item = e.Items.ToArray()[0] as Models.UI_Info_Str;
        }

        private void ListView_DragItemsCompleted(ListViewBase sender, DragItemsCompletedEventArgs args)
        {
            stb_del_out.Begin();
            _del_item = null;
        }

        private void gr_del_DragOver(object sender, DragEventArgs e)
        {
            e.AcceptedOperation = Windows.ApplicationModel.DataTransfer.DataPackageOperation.Move;
        }

        private void gr_del_Drop(object sender, DragEventArgs e)
        {
            if(_del_item!=null)
            {
                Items.Remove(_del_item);
            }
        }

        private void gr_del_DragLeave(object sender, DragEventArgs e)
        {
            stb_del_out.Begin();
        }

        private void gr_del_DragEnter(object sender, DragEventArgs e)
        {
            stb_del_in.Begin();
        }

        private void gr_del_DragStarting(UIElement sender, DragStartingEventArgs args)
        {
            
        }
        #endregion

        #region Add Button Fucntion
        private void gr_add_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            stb_add_in.Begin();
        }

        private void gr_add_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout((FrameworkElement)sender);
        }

        private void gr_add_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            stb_add_out.Begin();
        }

        private void gr_add_Tapped(object sender, TappedRoutedEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout((FrameworkElement)sender);
        }
        #endregion


    }
}
