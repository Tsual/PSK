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
        }

        Point _StartPoint;
        double _maxLen = 0;
        double _ActiveLen = 0;
        double _ActivePercent = 0;
        double _maxlen_m_actd = 0;
        bool isPullActive = true;
        Thickness _originmargin;
        Thickness _emptymargin = new Thickness();

        private void _root_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            _root.ManipulationDelta -= _root_ManipulationDelta;

            if (isPullActive)
            {
                _head_renderT_backA.From = _head_renderT.TranslateY;
                _head_renderT_backA1.From = _head_renderT.TranslateY;
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
                if(vto>0)
                {
                    var t = getEase(vto);
                    _head_renderT.TranslateY = t;
                    grp.Height = t;
                    isPullActive = true;
                }
                else
                {
                    _head_renderT.TranslateY = YTs;
                    _emptymargin.Top = _originmargin.Top + YTs;
                    _ContentPanel.Margin = _emptymargin;
                    isPullActive = false;
                }
            }
            else
            {
                _head_renderT.TranslateY = YTs;
                _emptymargin.Top = _originmargin.Top + YTs;
                _ContentPanel.Margin = _emptymargin;
                isPullActive = false;
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



    }
}
