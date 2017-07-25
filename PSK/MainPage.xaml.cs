using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using PSK.UserComponent;
using Windows.UI.Xaml.Navigation;
using System.Threading.Tasks;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace PSK
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

        private async void Button_ClickAsync(object sender, RoutedEventArgs e)
        {
            string _pid = TB_UserName.Text;
            if (_pid == "")
            {
                MessageDialog md = new MessageDialog("name can't be null", "Error");
                md.Commands.Add(new UICommand("ok"));
                await md.ShowAsync();
                return;
            }

            string _pwd = TB_Password.Password;
            if (_pwd == "")
            {
                MessageDialog md = new MessageDialog("password can't be null", "Error");
                md.Commands.Add(new UICommand("ok"));
                await md.ShowAsync();
                return;
            }


            LoginUser _lu = LoginUser.CreateObj(_pid, _pwd);
            if (CB_CreateNew.IsChecked == null ? false : (bool)CB_CreateNew.IsChecked)
            {
                _lu.UserNotFoundEvent += (obj) => { return LoginUser.UserNotFoundReceipt.Create; };
            }
            else
            {
                _lu.UserNotFoundEvent +=  (obj) =>
                {

                    Task.Run(async () =>
                   {
                       MessageDialog md = new MessageDialog("we can't find your info", "Error");
                       md.Commands.Add(new UICommand("ok"));
                       await md.ShowAsync();
                   });

                   return LoginUser.UserNotFoundReceipt.None;


               };
            }
            _lu.UserPwdVertifyFailEvent += async (obj) =>
            {
                MessageDialog md = new MessageDialog("password vertify failed", "Error");
                md.Commands.Add(new UICommand("ok"));
                await md.ShowAsync();
            };
            _lu.UserVertifyEvent += (obj) =>
            {
                Frame.Navigate(typeof(ctrltestpage));
            };
            _lu.TryLogin();

        }


    }
}
