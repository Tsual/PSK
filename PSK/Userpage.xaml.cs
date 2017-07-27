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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Core.Current.Unsubscribe();
            if (Frame.CanGoBack)
                Frame.GoBack();
        }

        private void _StackPanel_Loaded(object sender, RoutedEventArgs e)
        {
            var _list = Core.Current.CurrentUser.Recordings;
            if (_list.Count == 0)
            {
                TextBlock tb = new TextBlock() { Text = "no record", HorizontalAlignment = HorizontalAlignment.Center };
                _StackPanel.Children.Add(tb);
            }
            else
            {
                for (int i = 0; i < _list.Count; i++)
                {
                    var obj = _list[i];
                    DetailCtrl ctrl = new DetailCtrl() { InfoItem = obj, ItemIndex = i + 1 };
                    ctrl.LostFocus += Ctrl_LostFocus_killctrl;
                    _StackPanel.Children.Add(ctrl);
                }
            }
            Button_Click_1(null, null);


            //var ran = new Helper.RandomGenerator();
            //for (int i = 0; i < 1000; i++)
            //{
            //    ItemCollection.Add(new Info() { DetailName = ran.getRandomString(5), Detail = ran.getRandomString(30) });
            //}
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            foreach (var t in _StackPanel.Children.ToList())
            {
                (t as DetailCtrl)?.ChangeState();
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            foreach (var t in _StackPanel.Children.ToList())
            {
                ((DetailCtrl)t).ChangeState(false);
            }
            var info = new Models.Info() { Detail = "", DetailName = "", Switchbool = true };
            Core.Current.CurrentUser.Recordings.Add(info);
            DetailCtrl ctrl = new DetailCtrl() { InfoItem = info, ItemIndex = Core.Current.CurrentUser.Recordings.Count };
            ctrl.LostFocus += Ctrl_LostFocus;
            _StackPanel.Children.Add(ctrl);
            ctrl.Focus(FocusState.Pointer);

        }

        private void Ctrl_LostFocus(object sender, RoutedEventArgs e)
        {
            _AddNewDetailCtrl = sender as DetailCtrl;
        }

        private void Ctrl_LostFocus_killctrl(object sender, RoutedEventArgs e)
        {
            var ctrl = sender as DetailCtrl;
            if (ctrl != null)
            {
                if (ctrl.isEmpty)
                {
                    Core.Current.CurrentUser.Recordings.Remove(ctrl.InfoItem);
                    _StackPanel.Children.Remove(ctrl);
                }
            }
        }

        private DetailCtrl _AddNewDetailCtrl;

        private void Grid_GotFocus(object sender, RoutedEventArgs e)
        {
            var _name = (e.OriginalSource as FrameworkElement).Name;
            if (_AddNewDetailCtrl == null) return;

            if (!((e.OriginalSource as FrameworkElement).Parent as FrameworkElement).Parent.Equals(_AddNewDetailCtrl))
            {
                if (_AddNewDetailCtrl.isEmpty)
                {
                    Core.Current.CurrentUser.Recordings.Remove(_AddNewDetailCtrl.InfoItem);
                    _StackPanel.Children.Remove(_AddNewDetailCtrl);
                }
                else
                {
                    _AddNewDetailCtrl.LostFocus -= Ctrl_LostFocus;
                    _AddNewDetailCtrl.LostFocus += Ctrl_LostFocus_killctrl;
                    _AddNewDetailCtrl.ChangeState(false);
                }
            }
            _AddNewDetailCtrl = null;
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
    }
}
