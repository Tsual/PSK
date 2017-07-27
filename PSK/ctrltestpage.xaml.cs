using System;
using System.Collections.Generic;
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
using PSK.Models;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Storage.Pickers;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace PSK
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class ctrltestpage : Page
    {
        public ctrltestpage()
        {
            this.InitializeComponent();


        }

        private async System.Threading.Tasks.Task Button_ClickAsync(object sender, RoutedEventArgs e)
        {
            //DataPac dat = new DataPac()
            //{
            //    dRows = new DataPac.dRow[2] {
            //        new DataPac.dRow(){ str1="aa",str2="ff"},new DataPac.dRow(){ str1="3213123",str2="f3121312f"}
            //    },
            //    str1 = "ffff",
            //    str2 = "fdsafasf",
            //    tolen = "fdsafsadf"
            //};
            //var picker = new FileSavePicker();
            //picker.DefaultFileExtension = ".xml";
            //picker.FileTypeChoices.Add("XML", new List<String>() { ".xml" });
            //var sf = await picker.PickSaveFileAsync();
            //await (new DataPacManager(dat)).SerializeAsync(sf);
            //int a = 0;
            //var picker = new FileOpenPicker();
            //picker.FileTypeFilter.Add(".xml");
            //var sf = await picker.PickSingleFileAsync();
            //var t = await (new DataPacManager(null)).DeserializeAsync(sf);
            //int a = 0;

        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            await Button_ClickAsync(sender, e);
        }
    }
}
