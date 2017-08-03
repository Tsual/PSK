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

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace PSK
{
    public sealed partial class VisualCollection_t1 : UserControl
    {
        public enum InitModel
        {
            VerMulti, VerLine, HorLine, HorMulti
        }




        public VisualCollection_t1()
        {
            this.InitializeComponent();
            


        }

        private InitModel _Initmodel = 0;
        public InitModel Initmodel { get => _Initmodel; set => _Initmodel = value; }

        public IEnumerable<test_model> ItemsCollection { get; set; }
        List<UIElement> UIElements = new List<UIElement>();

        private void c_ScrollBar_Holding(object sender, HoldingRoutedEventArgs e)
        {

        }
    }
}
