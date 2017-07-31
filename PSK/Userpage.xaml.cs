using PSK.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Pickers;
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
    public sealed partial class Userpage : Page
    {
        public ObservableCollection<Info> ItemCollection = Core.Current.CurrentUser.Recordings;
        public string PID { get { return Core.Current.CurrentUser.PID; } }
        public Userpage()
        {
            this.InitializeComponent();

        }

        private async void Button_Click_3(object sender, RoutedEventArgs e)
        {
            ConfirmBTN.Content = new LoadingCtrl() { ContentString = "Processing" };
            var spicker = new FileSavePicker();
            spicker.FileTypeChoices.Add("XML", new List<String>() { ".xml" });
            var sfs = await spicker.PickSaveFileAsync();
            await DataPacManager.SerializeAsync(sfs, TB_Password.Password);
            if ((bool)Export_cb.IsChecked)
            {
                Core.Current.DeleteUser();
                Core.Current.Unsubscribe();
                Frame.Navigate(typeof(MainPage));
            }
        }

        private void TB_Password_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (TB_Password.Password != "") ConfirmBTN.IsEnabled = true;
            else ConfirmBTN.IsEnabled = false;
        }

        private void c_new_btn_Click(object sender, RoutedEventArgs e)
        {
            if (c_new_tb.Text == "")
                return;
            ItemCollection.Add(new Info() { DetailName = c_new_tb.Text, Detail = "" });
            c_new_flyout.Hide();
        }

        private void _ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0) return;
            var _info = e.AddedItems[0] as Info;
            if (_info != null)
                Core.Current.DetailPage_databridge = new UI_Info(_info);
            Frame.Navigate(typeof(DetailPage));




        }

        #region del func
        Info _del_item = null;
        private void gr_del_DragOver(object sender, DragEventArgs e)
        {
            e.AcceptedOperation = Windows.ApplicationModel.DataTransfer.DataPackageOperation.Move;
        }

        private void gr_del_Drop(object sender, DragEventArgs e)
        {
            if (_del_item != null)
                Core.Current.CurrentUser.Recordings.Remove(_del_item);
            stb_del_out.Begin();
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

        private void _ListView_DragItemsStarting(object sender, DragItemsStartingEventArgs e)
        {
            _del_item = e.Items.ToArray()[0] as Info;
        }

        private void _ListView_DragItemsCompleted(ListViewBase sender, DragItemsCompletedEventArgs args)
        {
            stb_del_out.Begin();
            _del_item = null;
        }
        #endregion
    }
}
