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
            //int a = 0;
            //var spicker = new FileSavePicker();
            //spicker.FileTypeChoices.Add("XML", new List<String>() { ".xml" });
            //var sfs = await spicker.PickSaveFileAsync();
            //await DataPacManager.SerializeAsync(sfs, "test");


            //using (APPDbContext db = new APPDbContext())
            //{
            //    foreach (var t in db.Users.ToList())
            //        db.Entry(t).State = Microsoft.EntityFrameworkCore.EntityState.Deleted;
            //    foreach (var t in db.Recordings.ToList())
            //        db.Entry(t).State = Microsoft.EntityFrameworkCore.EntityState.Deleted;
            //    db.SaveChanges();
            //}



            //var picker = new FileOpenPicker();
            //picker.FileTypeFilter.Add(".xml");
            //var sf = await picker.PickSingleFileAsync();
            //await DataPacManager.DeserializeAsync(sf, "test");


        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            await Button_ClickAsync(sender, e);
        }
    }
}
