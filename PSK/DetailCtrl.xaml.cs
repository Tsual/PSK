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
using PSK.Models;
using System.Diagnostics;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace PSK
{
    public sealed partial class DetailCtrl : UserControl
    {
        public Info InfoItem
        {
            get { return _InfoItem; }
            set
            {
                if (_InfoItem == null)
                {
                    _InfoItem = value;
                    if (_InfoItem.isSwitchChangedEventNull)
                    {
                        _InfoItem.SwitchChangedEvent += (info) =>
                        {
                            GR_Modify_tb1.IsReadOnly = !_InfoItem.Switchbool;
                            GR_Modify_tb2.IsReadOnly = !_InfoItem.Switchbool;
                        };
                    }
                }
                else
                {
                    _InfoItem = value;
                }
            }
        }
        private Info _InfoItem = null;

        public void ChangeState()
        {
            InfoItem.Switchbool = !InfoItem.Switchbool;
        }

        public void ChangeState(bool b)
        {
            InfoItem.Switchbool = b;
        }

        public int ItemIndex { get; set; }
        public bool isEmpty
        {
            get
            {
                if (GR_Modify_tb1.Text == "" && GR_Modify_tb2.Text == "") return true;
                else return false;
            }
        }

        public DetailCtrl()
        {
            this.InitializeComponent();
        }
    }
}
