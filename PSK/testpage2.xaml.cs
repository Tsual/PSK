using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    public sealed partial class testpage2 : Page
    {
        public testpage2()
        {
            this.InitializeComponent();
            _root.ManipulationStarted += _root_ManipulationStarted;
            _root.ManipulationCompleted += _root_ManipulationCompleted;



            _ScrollBar.Maximum = _StackPanel.Children.Count;
            _ScrollBar.Minimum = 1;
            _ScrollBar.Value = 1;
            _ScrollBar.ViewportSize = _scrollbar_vs;
        }

        double _scrollbar_vs = 4;
        Point _StartPoint;
        double _maxLen = 0;
        double _ActiveLen = 0;
        double _ActivePercent = 0;
        double _maxlen_m_actd = 0;
        bool isPullActive = true;
        Thickness _originmargin;
        Thickness _emptymargin = new Thickness();
        double _minMargin
        {
            get { return _root.ActualHeight - _StackPanel.ActualHeight; }
        }

        private void _root_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            _root.ManipulationDelta -= _root_ManipulationDelta;

            if (isPullActive)
            {
                _head_renderT_backA.From = _head_renderT.TranslateY;
                _head_renderT_backA1.From = _head_renderT.TranslateY;
                _head_renderT_backA3.From = _ScrollBar_renderT.ScaleY;
                _head_renderT_backA3.To = 1;
                _head_renderT_backStb.Begin();
            }
            //Debug.WriteLine(_ContentPanel.Margin.Left + "  " + _ContentPanel.Margin.Top + "  " + _ContentPanel.Margin.Right + "  " + _ContentPanel.Margin.Bottom);
        }

        private double getEase(double len)
        {
            if (len < _ActiveLen) return len;
            else if (len < _maxLen) return _ActiveLen + (len - _ActiveLen) * _ActivePercent;
            else return _ActiveLen + _maxlen_m_actd * _ActivePercent + (len - _maxLen) * 0.1;
        }

        private void _root_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            var YTs = e.Position.Y - _StartPoint.Y;

            if (YTs > 0 && _originmargin.Top <= 0)
            {
                var vto = YTs + _originmargin.Top;
                if (vto > 0)
                {
                    //pulling head
                    var t = getEase(vto);
                    _head_renderT.TranslateY = t;
                    grp.Height = t;
                    isPullActive = true;
                    _ScrollBar_renderT.ScaleY = (t * t) / (vto * vto);
                }
                else
                {
                    //pulling but not attach head
                    _head_renderT.TranslateY = YTs;
                    _emptymargin.Top = _originmargin.Top + YTs;
                    _ContentPanel.Margin = _emptymargin;
                    isPullActive = false;
                    _ScrollBar.Value = (_emptymargin.Top * _ScrollBar.Maximum) / _minMargin;
                }
            }
            else
            {
                //drap
                double _MinMargin = _minMargin;
                if (_originmargin.Top + YTs < _minMargin) return;
                _head_renderT.TranslateY = YTs;
                _emptymargin.Top = _originmargin.Top + YTs;
                _ContentPanel.Margin = _emptymargin;
                isPullActive = false;
                _ScrollBar.Value = (_emptymargin.Top * _ScrollBar.Maximum) / _MinMargin;
            }
        }

        private void _root_ManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {
            _originmargin = _ContentPanel.Margin;
            _StartPoint = e.Position;
            _maxLen = 300;
            _ActivePercent = 0.4;
            _ActiveLen = _maxLen * _ActivePercent;
            _maxlen_m_actd = _maxLen - _ActiveLen;
            _root.ManipulationDelta += _root_ManipulationDelta;
        }

        int PointerWheelChanged_index = 35;
        private void _root_PointerWheelChanged(object sender, PointerRoutedEventArgs e)
        {
            if (isPullActive && _ContentPanel.Margin.Top > 0) return;
            double position = _ContentPanel.Margin.Top + PointerWheelChanged_index * e.GetCurrentPoint(sender as UIElement).Properties.MouseWheelDelta.CompareTo(0);
            if (position > 0) position = 0;
            double _MinMargin = _minMargin;
            if (_MinMargin == 0) return;
            if (position < _MinMargin) position = _MinMargin;



            _head_renderT.TranslateY = position;
            _emptymargin = new Thickness() { Top = position };
            _ContentPanel.Margin = _emptymargin;

            _ScrollBar.Value = (position * _ScrollBar.Maximum) / _MinMargin;
        }



    }
}
