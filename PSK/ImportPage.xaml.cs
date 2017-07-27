using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Pickers;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using PSK.Models;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace PSK
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class ImportPage : Page
    {
        public ImportPage()
        {
            this.InitializeComponent();
        }

        private StorageFile sf;

        private async void SelectFileBTN_Click(object sender, RoutedEventArgs e)
        {
            var picker = new FileOpenPicker();
            picker.FileTypeFilter.Add(".xml");
            sf = await picker.PickSingleFileAsync();
            if (sf != null)
                Filename_tb.Text = sf.Name;
        }

        private async void ConfirmBTN_Click(object sender, RoutedEventArgs e)
        {
            ConfirmBTN.Content = new LoadingCtrl();
            try
            {
                await DataPacManager.DeserializeAsync(sf, TB_Password.Password);
                Frame.Navigate(typeof(MainPage));
            }
            catch (DataPacManager.KeyVertifyFailException)
            {
                TB_Password.BorderBrush = new SolidColorBrush(new Windows.UI.Color() { A = 255, R = 255, B = 0, G = 0 });
                TB_Password.Header = new TextBlock() { Text = "Password error", Foreground = new SolidColorBrush(new Windows.UI.Color() { A = 255, R = 255, B = 0, G = 0 }) };
                ConfirmBTN.Content = "Submit";
            }
        }

        private void TB_Password_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (TB_Password.Password != "" && sf != null) ConfirmBTN.IsEnabled = true;
            else ConfirmBTN.IsEnabled = false;
        }
    }
}
