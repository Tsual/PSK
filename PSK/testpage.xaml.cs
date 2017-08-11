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
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;
using System.Collections.ObjectModel;
using Microsoft.Toolkit.Uwp;
using System.Diagnostics;


// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板s

namespace PSK
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class testpage : Page
    {
        public string prot1 { get { return Rect_at.ActualWidth + "\\" + Rect_at.ActualHeight; } }
        public string b_str { get { return b_str_var; } }
        string b_str_var = "";
        //return Rect_at.TransformToVisual(Window.Current.Content).TransformPoint(new Point(0, 0)).Y.ToString();


        ObservableCollection<testpage_itemModel> _itemCollection = new ObservableCollection<testpage_itemModel>();


        public Rectangle rect { get { return Rect_at; } }


        public testpage()
        {
            this.InitializeComponent();

            RefreshIndicator.SizeChanged += RefreshIndicator_SizeChanged;

            zp = new Point(0, 0);
            gt = Rect_at.TransformToVisual(Window.Current.Content);
            for(int i=0;i<100;i++)
            {
                _itemCollection.Add(new testpage_itemModel() { Str = i + i + ".." });
            }


        }

        private void RefreshIndicator_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            RefreshIndicatorTransform.TranslateY = -RefreshIndicator.ActualHeight;
            Debug.WriteLine("<<Token>>" + e.NewSize.Height + "::" + e.NewSize.Width);
        }

        Point zp ;
        GeneralTransform gt ;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var t = Rect_at.TransformToVisual(_ScrollViewer).TransformPoint(new Point(0, 0)).Y.ToString();
            var t1 = Rect_at.TransformToVisual(Window.Current.Content).TransformPoint(new Point(0, 0)).Y.ToString();
            var t2=_StackPanel.TransformToVisual(Window.Current.Content).TransformPoint(new Point(0, 0)).Y.ToString();
            int a = 0;
        }

        private void _ScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            b_str_var = Rect_at.TransformToVisual(_ScrollViewer).TransformPoint(new Point(0, 0)).Y.ToString();
        }

        private void _LIstVIew_ItemClick(object sender, ItemClickEventArgs e)
        {
            
        }

        private void _ptrlv_RefreshRequested(object sender, EventArgs e)
        {
            Debug.WriteLine("<<Fucntion>>exec:_ptrlv_RefreshRequested");
        }

        private void _ptrlv_PullProgressChanged(object sender, Microsoft.Toolkit.Uwp.UI.Controls.RefreshProgressEventArgs e)
        {
            //Debug.WriteLine("<<Fucntion>>exec:_ptrlv_PullProgressChanged");
            _RefreshIndicatorContent.Text = e.PullProgress.ToString();
        }

        private void _ptrlv_RefreshIntentCanceled(object sender, EventArgs e)
        {
            Debug.WriteLine("<<Fucntion>>exec:_ptrlv_RefreshIntentCanceled");
        }
    }

    public class testpage_itemModel
    {
        public string Str { get; set; }
    }
}
