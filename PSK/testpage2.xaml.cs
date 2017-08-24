using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System.Threading;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Collections.ObjectModel;

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
            _PanelInit();
            DCm_Init();
            ItemCollectionInit();
            testFuncs();
        }

        #region 视图组件


        private void _PanelInit()
        {
            _root.ManipulationStarted += _root_ManipulationStarted;
            _root.ManipulationCompleted += _root_ManipulationCompleted;

            DataItems.CollectionChanged += Items_CollectionChanged;

            _ScrollBar.Minimum = 1;
            _ScrollBar.Value = 1;
            _ScrollBar.ViewportSize = _scrollbar_vs;

            DataItems.CollectionChanged += ItemsFirstAdd;
        }

        private void ItemsFirstAdd(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            this.AddItems(DataItems[0].ControlType, _DCm_pac.Loaded_count + 4 * _DCm_pac.Buffer_count);
            DataItems.CollectionChanged -= ItemsFirstAdd;
            this.RefreshWall();
        }

        private void _PanelRefresh()
        {

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
            get { return _root.ActualHeight - ButtomWall; }
        }

        //私有刷新请求事件
        private enum ImpactDirection { Top, Buttom };
        private delegate IEnumerable<DCm_CtrlOrder> IndexerImpactEvent(ImpactDirection Direction);
        private event IndexerImpactEvent IndexerImpacted;

        private double ButtomWall
        {
            get
            {
                return _ButtomWall;
            }
            set
            {
                var obj = FindGridByIndex(_DCm_pac.Buttom_buffered_buttom);
                if (obj.RenderTransform is CompositeTransform CompositeTransform1)
                    _ButtomWall= CompositeTransform1.TranslateY + obj.ActualHeight;
            }
        }
        private double TopWall
        {
            get
            {
                return _TopWall;
            }
            set
            {
                var obj = FindGridByIndex(_DCm_pac.Loaded_top);
                if (obj.RenderTransform is CompositeTransform CompositeTransform1)
                    _TopWall= CompositeTransform1.TranslateY;
            }
        }

        private double _ButtomWall = 0;
        private double _TopWall = 0;

        private void RefreshWall()
        {
            ButtomWall = 0;
            TopWall = 0;
        }

        private void Items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            _ListCount = DataItems.Count;
        }

        /// <summary>
        /// 下拉缓动算法
        /// </summary>
        /// <param name="len">下拉距离</param>
        /// <returns></returns>
        private double getEase(double len)
        {
            if (len < _ActiveLen) return len;
            else if (len < _maxLen) return _ActiveLen + (len - _ActiveLen) * _ActivePercent;
            else return _ActiveLen + _maxlen_m_actd * _ActivePercent + (len - _maxLen) * 0.1;
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
                isPullActive = false;
                ControlOrderDelta(IndexerImpacted?.Invoke(ImpactDirection.Top));
            }
            //Debug.WriteLine(_ContentPanel.Margin.Left + "  " + _ContentPanel.Margin.Top + "  " + _ContentPanel.Margin.Right + "  " + _ContentPanel.Margin.Bottom);
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
                    if (_originmargin.Top + YTs > -TopWall)
                    {
#pragma warning disable CS4014 // 由于此调用不会等待，因此在调用完成前将继续执行当前方法
                        Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, () =>
                        {
                            ControlOrderDelta(IndexerImpacted?.Invoke(ImpactDirection.Top));
                        });
#pragma warning restore CS4014 // 由于此调用不会等待，因此在调用完成前将继续执行当前方法
                        return;
                    }

                    _head_renderT.TranslateY = YTs;
                    _emptymargin.Top = _originmargin.Top + YTs;
                    _ContentPanel.Margin = _emptymargin;
                    isPullActive = false;
                    Set_PointerWheel_Value((_emptymargin.Top * _ScrollBar.Maximum) / _minMargin);
                }
            }
            else
            {
                //下拉组函数
                double _MinMargin = _minMargin;

                //下拉越过边界
                if (_originmargin.Top + YTs < _MinMargin)
                {
                    if (_DCm_pac.Buttom_buffered_buttom == DataItems.Count || _DCm_pac.Loaded_buttom == DataItems.Count) return;
#pragma warning disable CS4014 // 由于此调用不会等待，因此在调用完成前将继续执行当前方法
                    Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, () =>
                    {
                        ControlOrderDelta(IndexerImpacted?.Invoke(ImpactDirection.Buttom));
                    });
#pragma warning restore CS4014 // 由于此调用不会等待，因此在调用完成前将继续执行当前方法
                    return;
                }

                _head_renderT.TranslateY = YTs;
                _emptymargin.Top = _originmargin.Top + YTs;
                _ContentPanel.Margin = _emptymargin;
                isPullActive = false;
                Set_PointerWheel_Value((_emptymargin.Top * _ScrollBar.Maximum) / _MinMargin);
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

        //滚轮滚动
        int PointerWheelChanged_index = 35;
        private void _root_PointerWheelChanged(object sender, PointerRoutedEventArgs e)
        {
            if (isPullActive && _ContentPanel.Margin.Top > 0) return;
            double position = _ContentPanel.Margin.Top + PointerWheelChanged_index * e.GetCurrentPoint(sender as UIElement).Properties.MouseWheelDelta.CompareTo(0);
            if (position > 0) position = 0;
            double _MinMargin = _minMargin;
            if (_MinMargin == 0) return;

                


            if (position < _MinMargin)
            {
                ControlOrderDelta(IndexerImpacted?.Invoke(ImpactDirection.Buttom));
                position = _MinMargin;
            }

            if (position >= -TopWall)
                ControlOrderDelta(IndexerImpacted?.Invoke(ImpactDirection.Top));


            _head_renderT.TranslateY = position;
            _emptymargin = new Thickness() { Top = position };
            _ContentPanel.Margin = _emptymargin;

            Set_PointerWheel_Value((position * _ScrollBar.Maximum) / _MinMargin);
        }

        double dl = 1;
        private void Set_PointerWheel_DL(double dl)
        {
            this.dl = dl;
        }
        private void Set_PointerWheel_Value(double value)
        {
            _ScrollBar.Value = value / dl;
        }

        List<DCm_CtrlOrder> Ord_move = new List<DCm_CtrlOrder>();
        List<DCm_CtrlOrder> Ord_load = new List<DCm_CtrlOrder>();



        /// <summary>
        /// 控件指令处理方法
        /// </summary>
        /// <param name="orders">控件指令列表</param>
        private void ControlOrderDelta(IEnumerable<DCm_CtrlOrder> orders)
        {
            if (orders == null) return;
            Ord_move.Clear();
            Ord_load.Clear();
            foreach (var t in orders)
            {
                switch (t.OrderType)
                {
                    case DCm_CtrlOrder.CtrlOrderType.Load:
                        Ord_load.Add(t);
                        break;
                    case DCm_CtrlOrder.CtrlOrderType.LoadOrCreate:
                        Ord_load.Add(t);
                        break;
                    case DCm_CtrlOrder.CtrlOrderType.Move:
                        int index = -1;
                        for (int i = 0; i < Ord_move.Count; i++)
                        {
                            if (Ord_move[i].Target > t.Target)
                            {
                                index = i;
                                break;
                            }
                        }
                        if (index > 0)
                            Ord_move.Insert(index, t);
                        else
                            Ord_move.Add(t);
                        break;
                }
            }


            if (Ord_move.Count > 0)
            {
                ImpactDirection dire = Ord_move[0].Origin < Ord_move[0].Target ? ImpactDirection.Buttom : ImpactDirection.Top;
                for (int i = 0; i < Ord_move.Count; i++)
                {
                    if (dire == ImpactDirection.Buttom)
                    {
                        var _target = FindGridByIndex(Ord_move[i].Origin);
                        var _tge = FindGridByIndex(Ord_move[i].Target - 1);
                        if (_target.RenderTransform is CompositeTransform _CompositeTransform1)
                        {
                            if (_tge.RenderTransform is CompositeTransform _CompositeTransform2)
                            {
                                _CompositeTransform1.TranslateY = _CompositeTransform2.TranslateY + _target.ActualHeight;
                                _ContentPanel.Height += _target.ActualHeight;
                                FindBinderByGrid(_target)._Index = Ord_move[i].Target;
                            }
                        }
                    }
                    else
                    {
                        var list = DCm_Binder.list;
                        var _target = FindGridByIndex(Ord_move[Ord_move.Count - i - 1].Origin);
                        var _tge = FindGridByIndex(Ord_move[Ord_move.Count - i - 1].Target + 1);
                        if (_target.RenderTransform is CompositeTransform _CompositeTransform1)
                        {
                            if (_tge.RenderTransform is CompositeTransform _CompositeTransform2)
                            {
                                _CompositeTransform1.TranslateY = _CompositeTransform2.TranslateY - _target.ActualHeight;
                                FindBinderByGrid(_target)._Index = Ord_move[Ord_move.Count - i - 1].Target;
                            }
                        }
                    }
                }
            }


            if (Ord_load.Count > 0)
            {
                foreach (var t in Ord_load)
                {
                    LoadData(DataItems[t.Target - 1], FIndControlByIndex(t.Target));
                }
                //var obj1 = FindGridByIndex(_DCm_pac.Loaded_top);
                //if (obj1 != null)
                //    if (obj1.RenderTransform is CompositeTransform _CompositeTransform1)
                //        _SetTopWall(_CompositeTransform1.TranslateY);
                //var obj2 = FindGridByIndex(_DCm_pac.Loaded_buttom);
                //if (obj2 != null)
                //    if (obj2.RenderTransform is CompositeTransform _CompositeTransform2)
                //        _SetButtomWall(_CompositeTransform2.TranslateY + obj1.ActualHeight);
            }


            this.RefreshWall();


        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            InitDefaultControlMargin();
        }

        #region 辅助方法

        private iDataControl FIndControlByIndex(int Index)
        {
            foreach (var t in _ContentPanel.Children)
            {
                if (t is Grid ins1)
                {
                    if (VisualTreeHelper.GetChildrenCount(ins1) < 1) return null;
                    if (ins1.Children[0] is iDataControl ins2)
                    {
                        var ctrl = DCm_Binder.FindPacByControl(ins2);
                        if (ctrl != null)
                            if (ctrl._Index == Index)
                                return ins2;
                    }
                }
            }
            return null;
        }

        private Grid FindGridByDire(ImpactDirection Dire)
        {
            if (VisualTreeHelper.GetChildrenCount(_ContentPanel) < 1) return null;
            Grid _target = null;
            switch (Dire)
            {
                case ImpactDirection.Top:
                    _target = _ContentPanel.Children[0] as Grid;
                    for (int i = 1; i < _ContentPanel.Children.Count; i++)
                    {
                        if (_ContentPanel.Children[i] is Grid _grid)
                        {
                            if (_grid.RenderTransform is CompositeTransform _CompositeTransform)
                            {
                                if (_target.RenderTransform is CompositeTransform _CompositeTransform1)
                                {
                                    if (_CompositeTransform.TranslateY > _CompositeTransform1.TranslateY)
                                        _target = _grid;
                                }
                            }
                        }
                    }
                    break;
                case ImpactDirection.Buttom:
                    _target = _ContentPanel.Children[0] as Grid;
                    for (int i = 1; i < _ContentPanel.Children.Count; i++)
                    {
                        if (_ContentPanel.Children[i] is Grid _grid)
                        {
                            if (_grid.RenderTransform is CompositeTransform _CompositeTransform)
                            {
                                if (_target.RenderTransform is CompositeTransform _CompositeTransform1)
                                {
                                    if (_CompositeTransform.TranslateY < _CompositeTransform1.TranslateY)
                                        _target = _grid;
                                }
                            }
                        }
                    }
                    break;
            }
            return _target;
        }

        private int FindIndexByGrid(Grid _Grid)
        {
            if (VisualTreeHelper.GetChildrenCount(_Grid) < 1) return -1;
            if (_Grid.Children[0] is iDataControl ins1)
            {
                return DCm_Binder.FindPacByControl(ins1)._Index;
            }
            return -1;
        }

        private Grid FindGridByIndex(int Index)
        {
            foreach (var t in _ContentPanel.Children)
            {
                if (t is Grid ins1)
                {
                    if (VisualTreeHelper.GetChildrenCount(ins1) < 1) continue;
                    if (ins1.Children[0] is iDataControl ins2)
                    {
                        var obj = DCm_Binder.FindPacByControl(ins2);
                        if (obj != null)
                            if (obj._Index == Index)
                                return ins1;
                    }
                }
            }
            return null;
        }

        private DCm_Binder.DCm_BinderPac FindBinderByGrid(Grid _Grid)
        {
            if (VisualTreeHelper.GetChildrenCount(_Grid) < 1) return null;
            if (_Grid.Children[0] is iDataControl ins1)
            {
                return DCm_Binder.FindPacByControl(ins1);
            }
            return null;
        }

        public int ActiveDestroyDelay { get { return _ActiveDestroyDelay; } set { _ActiveDestroyDelay = value; } }
        private int _ActiveDestroyDelay = 3;

        /// <summary>
        /// 载入数据 异步
        /// </summary>
        /// <param name="Model">数据</param>
        /// <param name="Control">控件</param>
        private void LoadData(VirtualModel Model, iDataControl Control)
        {
            if (Control == null || Model == null) return;
            ThreadPoolTimer _timer1 = null;
            ThreadPoolTimer _timer2 = null;
            _timer1 = ThreadPoolTimer.CreateTimer(async (e) =>
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, () =>
                {
                    Control.Load(Model);
                });
                _timer2?.Cancel();
            }, new TimeSpan(0));
            _timer2 = ThreadPoolTimer.CreateTimer((e) =>
            {
                _timer1?.Cancel();
            }, TimeSpan.FromSeconds(ActiveDestroyDelay));
        }

        /// <summary>
        /// 创建模板控件组 在一个root生命周期里只能创建一次
        /// </summary>
        /// <param name="ControlType">控件类型</param>
        /// <param name="count">创建数量</param>
        private void AddItems(Type ControlType, int count)
        {
            if (_ContentPanel.Children.Count > 0) return;
            for (int i = 0; i < count; i++)
            {
                var obj = Activator.CreateInstance(ControlType);
                if (obj is iDataControl ins1)
                {
                    ins1.Default();
                    if (obj is UIElement ins2)
                    {
                        Grid _grid = new Grid();
                        _grid.Children.Add(ins2);
                        _grid.RenderTransform = new CompositeTransform();
                        _grid.VerticalAlignment = VerticalAlignment.Top;
                        _ContentPanel.Children.Add(_grid);
                        DCm_Binder.Execute(ins2 as iDataControl, null, i + 1);
                    }

                }

            }

        }

        private void InitDefaultControlMargin()
        {
            double _margin_top = 0;
            for (int i = 0; i < _ContentPanel.Children.Count; i++)
            {
                if (_ContentPanel.Children[i] is Grid _grid)
                {
                    if (_grid.RenderTransform is CompositeTransform _CompositeTransform)
                        _CompositeTransform.TranslateY = _margin_top;
                    _margin_top += _grid.ActualHeight;
                }
                _ContentPanel.Height = _margin_top;
            }
            ControlOrderDelta(_DCm_pac.DCm_pac_refresh());
        }








        //数据加载模板
        //var _ty = t.ControlType;
        //var obj = Activator.CreateInstance(_ty);
        //(obj as iDataControl).Load(t as s_Data_virtual);
        //_StackPanel.Children.Add(obj as UIElement);


        //测试模型
        #endregion


        #endregion

        #region 数据控制组件 核心数据类在这 在这！
        private ObservableCollection<VirtualModel> _DataItems = new ObservableCollection<VirtualModel>();
        public ObservableCollection<VirtualModel> DataItems { get => _DataItems; set => _DataItems = value; }

        /// <summary>
        /// 私有数据聚合类 包含数据标识重定位方法
        /// </summary>
        private class DCm_pac
        {
            public void tes()
            {
                for (int i = 0; i < 10; i++)
                    Push(ImpactDirection.Buttom);
            }

            private int[] _data = new int[13] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
            private ObservableCollection<VirtualModel> _targetlist = null;

            #region 构造

            public DCm_pac(int loaded, int buffersize, ObservableCollection<VirtualModel> list)
            {
                Loaded_count = loaded;
                Empty_count = buffersize;
                Buffer_count = buffersize;
                this._targetlist = list;
                list.CollectionChanged += List_CollectionChanged;
            }

            /// <summary>
            /// 最简构造 20 3 3
            /// </summary>
            public DCm_pac(ObservableCollection<VirtualModel> list)
            {
                Loaded_count = 20;
                Empty_count = 3;
                Buffer_count = 3;
                this._targetlist = list;
                list.CollectionChanged += List_CollectionChanged;
            }

            private void List_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
            {
                Relocate(_data[7]);
            }
            #endregion

            //public void pri()
            //{

            //    Debug.WriteLine("----------------------------");
            //    Debug.WriteLine(Top_empty_top + " " + Top_empty_buttom);
            //    Debug.WriteLine(Top_buffered_top + " " + Top_buffered_buttom);
            //    Debug.WriteLine(Loaded_top + " " + Loaded_buttom);
            //    Debug.WriteLine(Buttom_buffered_top + " " + Buttom_buffered_buttom);
            //    Debug.WriteLine(Buttom_empty_top + " " + Buttom_empty_buttom);
            //}

            /// <summary>
            /// 发布初始化命令
            /// </summary>
            /// <returns>控件命令列表</returns>
            public IEnumerable<DCm_CtrlOrder> DCm_pac_refresh()
            {
                return Relocate(_data[7]);
            }
            /// <summary>
            /// list.Count-load-buffer>Loaded_top_index>0
            /// </summary>
            private IEnumerable<DCm_CtrlOrder> Relocate(int Loaded_top_index)
            {
                //入参规则
                if (Loaded_top_index + this._data[0] > _targetlist.Count
                    || Loaded_top_index < 1
                    || _targetlist.Count < 1) return null;

                //创建局部数据镜像
                int[] _data = new int[13];
                this._data.CopyTo(_data, 0);
                //计算下半区位移
                _data[7] = Loaded_top_index;
                _data[8] = _data[7] + Loaded_count - 1;
                _data[9] = _data[8] + 1;
                _data[10] = _data[9] + Buffer_count - 1;
                _data[11] = _data[10] + 1;
                _data[12] = _data[11] + Empty_count - 1;
                //位移规则修正
                bool token = false;
                for (int i = 8; i < 13; i++)
                    if (_data[i] > _targetlist.Count || token)
                    {
                        _data[i] = _targetlist.Count;
                        token = true;
                    }
                //计算上半区唯一
                _data[6] = _data[7] - 1;
                _data[5] = _data[6] - Buffer_count + 1;
                _data[4] = _data[5] - 1;
                _data[3] = _data[4] - Empty_count + 1;
                //位移规则修正
                token = false;
                for (int i = 6; i > 2; i--)
                    if (_data[i] < 1 || token)
                    {
                        _data[i] = 1;
                        token = true;
                    }
                //控件指令
                var orderlist = new List<DCm_CtrlOrder>();
                if (Loaded_top_index == this._data[7])//域变化或初始化时 重新排版
                {
                    for (int i = _data[5]; i <= _data[10]; i++)
                    {
                        orderlist.Add(new DCm_CtrlOrder()
                        {
                            Target = i,
                            OrderType = DCm_CtrlOrder.CtrlOrderType.LoadOrCreate
                        });
                    }
                    for (int i = _data[3]; i < _data[5]; i++)
                    {
                        orderlist.Add(new DCm_CtrlOrder()
                        {
                            Target = i,
                            OrderType = DCm_CtrlOrder.CtrlOrderType.Create
                        });
                    }
                    for (int i = _data[12]; i > _data[10]; i--)
                    {
                        orderlist.Add(new DCm_CtrlOrder()
                        {
                            Target = i,
                            OrderType = DCm_CtrlOrder.CtrlOrderType.Create
                        });
                    }



                }
                else if (Loaded_top_index > this._data[7])//向下移动
                {
                    //移动上部empty
                    List<int> __emptytt = new List<int>();
                    for (int i = this._data[3]; i < _data[3]; i++) __emptytt.Add(i);
                    //拼接下部empty
                    List<int> __emptybb = new List<int>();
                    for (int i = this._data[12] + 1; i <= _data[12]; i++) __emptybb.Add(i);
                    //获取更新区
                    List<int> __buffer = new List<int>();
                    for (int i = this._data[11]; i <= _data[10]; i++) __buffer.Add(i);



                    for (int i = 0; i < _data[1]; i++)
                    {
                        orderlist.Add(new DCm_CtrlOrder()
                        {
                            Origin = i >= __emptytt.Count ? -1 : __emptytt[i],
                            Target = i >= __emptybb.Count ? -2 : __emptybb[i],
                            OrderType = DCm_CtrlOrder.CtrlOrderType.Move
                        });

                    }
                    foreach (var t in __buffer)
                        orderlist.Add(new DCm_CtrlOrder()
                        {
                            Target = t,
                            OrderType = DCm_CtrlOrder.CtrlOrderType.Load
                        });



                }
                else//向上移动
                {
                    //上部empty
                    List<int> __emptytt = new List<int>();
                    for (int i = _data[3]; i < this._data[3]; i++) __emptytt.Add(i);
                    //下部empty
                    List<int> __emptybb = new List<int>();
                    for (int i = _data[12] + 1; i <= this._data[12]; i++) __emptybb.Add(i);
                    //获取更新区
                    List<int> __buffer = new List<int>();
                    for (int i = _data[5]; i <= this._data[4]; i++) __buffer.Add(i);



                    for (int i = 0; i < _data[1]; i++)
                    {
                        orderlist.Add(new DCm_CtrlOrder()
                        {
                            Origin = i >= __emptybb.Count ? -1 : __emptybb[i],
                            Target = i >= __emptytt.Count ? -2 : __emptytt[i],
                            OrderType = DCm_CtrlOrder.CtrlOrderType.Move
                        });
                    }

                    foreach (var t in __buffer)
                        orderlist.Add(new DCm_CtrlOrder()
                        {
                            Target = t,
                            OrderType = DCm_CtrlOrder.CtrlOrderType.Load
                        });
                }
                //验证指令 验证是否超域
                List<DCm_CtrlOrder> reml = new List<DCm_CtrlOrder>();
                foreach (var t in orderlist)
                {
                    if (t.OrderType == DCm_CtrlOrder.CtrlOrderType.Move)
                    {
                        if (t.Target == -2) reml.Add(t);
                        else if (t.Origin == -1) t.OrderType = DCm_CtrlOrder.CtrlOrderType.Create;
                    }
                }
                foreach (var t in reml)
                    orderlist.Remove(t);
                //应用数据并返回
                _data.CopyTo(this._data, 0);
                return orderlist;





            }

            /// <summary>
            /// 计算数据标识 移动Buffer_count
            /// <param name="Direction">滑窗移动方向</paramref>
            /// </summary>
            public IEnumerable<DCm_CtrlOrder> Push(ImpactDirection Direction)
            {
                return Push(Direction, _data[1]);
            }

            /// <summary>
            /// 计算数据标识 
            /// <param name="Direction">滑窗移动方向</paramref>
            /// <param name="Length">大于0且不大于Buffer_count</paramref>
            /// </summary>
            public IEnumerable<DCm_CtrlOrder> Push(ImpactDirection Direction, int Length)
            {
                if (Length < 0 || Length > _data[2]) return null;
                switch (Direction)
                {
                    case ImpactDirection.Buttom:
                        int miv = (_data[7] + Length) > _targetlist.Count ? _targetlist.Count : _data[7] + Length;
                        return Relocate(miv);
                    case ImpactDirection.Top:
                        miv = (_data[7] - Length) > 0 ? _data[7] - Length : 1;
                        return Relocate(miv);
                }
                return null;
            }

            #region 公开属性
            /// <summary>
            /// 装载总数
            /// </summary>
            public int Loaded_count { get => _data[0]; set => _data[0] = value; }

            /// <summary>
            ///上下各core_empty个
            ///</summary>
            public int Empty_count { get => _data[1]; set => _data[1] = value; }

            /// <summary>
            ///预加载数量
            ///</summary>
            public int Buffer_count { get => _data[2]; set => _data[2] = value; }

            public int Top_empty_top { get => _data[3]; set => _data[3] = value; }
            public int Top_empty_buttom { get => _data[4]; set => _data[4] = value; }

            public int Top_buffered_top { get => _data[5]; set => _data[5] = value; }
            public int Top_buffered_buttom { get => _data[6]; set => _data[6] = value; }

            public int Loaded_top { get => _data[7]; set => _data[7] = value; }
            public int Loaded_buttom { get => _data[8]; set => _data[8] = value; }

            public int Buttom_buffered_top { get => _data[9]; set => _data[9] = value; }
            public int Buttom_buffered_buttom { get => _data[10]; set => _data[10] = value; }

            public int Buttom_empty_top { get => _data[11]; set => _data[11] = value; }
            public int Buttom_empty_buttom { get => _data[12]; set => _data[12] = value; }
            #endregion
        }

        /// <summary>
        /// 控件操作指令
        /// </summary>
        private class DCm_CtrlOrder
        {
            public int Origin { get; set; }
            public int Target { get; set; }
            public CtrlOrderType OrderType { get; set; }
            public enum CtrlOrderType { Create, Move, Load, LoadOrCreate }
            public override string ToString()
            {
                return Origin + " " + Target + " " + OrderType;
            }
        }

        DCm_pac _DCm_pac = null;
        private int _ListCount { get; set; }

        private void DCm_Init()
        {
            this.IndexerImpacted += _IndexerImpacted;
            _DCm_pac = new DCm_pac(20, 5, _DataItems);
        }

        private IEnumerable<DCm_CtrlOrder> _IndexerImpacted(ImpactDirection Direction)
        {
            return _DCm_pac.Push(Direction);
        }

        #endregion

        private static class DCm_Binder
        {
            public class DCm_BinderPac
            {
                public iDataControl control = null;
                public VirtualModel model = null;
                public int _Index = 0;
                public override string ToString()
                {
                    return control.GetHashCode() + "|" + model.GetHashCode() + "|" + _Index;
                }
            }
            public static List<DCm_BinderPac> list = new List<DCm_BinderPac>();
            public static void Execute(iDataControl control, VirtualModel model, int index)
            {
                if (control == null) return;
                DCm_BinderPac obj = null;
                foreach (var t in list)
                {
                    if (t.control == control)
                    {
                        obj = t;
                        break;
                    }
                }
                if (obj == null)
                    list.Add(new DCm_BinderPac() { control = control, model = model, _Index = index });
                else
                {
                    obj.model = model;
                    obj._Index = index;
                }
            }
            public static DCm_BinderPac FindPacByControl(iDataControl control)
            {
                foreach (var t in list)
                    if (t.control == control)
                        return t;
                return null;
            }
            public static iDataControl FindControl(VirtualModel model)
            {
                if (model == null) return null;
                foreach (var t in list)
                    if (t.model == model)
                        return t.control;
                return null;
            }
            public static iDataControl FindControl(int index)
            {
                foreach (var t in list)
                    if (t._Index == index)
                        return t.control;
                return null;
            }
        }

        #region 模板测试类
        public class s_Data : VirtualModel
        {
            public int index { get; set; }
            public byte DA { get; set; }
            public byte DR { get; set; }
            public byte DB { get; set; }
            public byte DG { get; set; }

            public override Type ControlType => typeof(s_Control);
        }
        public class s_Control : Grid, iDataControl
        {
            public VirtualModel Data => _data;
            VirtualModel _data = null;

            public void Load(VirtualModel data)
            {
                if (data is s_Data data_m)
                {
                    Background = new SolidColorBrush(new Windows.UI.Color() { A = data_m.DA, B = data_m.DB, G = data_m.DG, R = data_m.DR });

                    if (VisualTreeHelper.GetChildrenCount(this) > 0)
                    {
                        foreach (var t in Children)
                            if (t is TextBlock tb1)
                                tb1.Text = "" + data_m.index;
                    }
                    else
                    {
                        TextBlock tb = new TextBlock() { Text = "" + data_m.index };
                        this.Children.Add(tb);
                    }
                    Margin = new Thickness(10);
                    Height = 50;
                    Width = 200;
                    _data = data_m;
                }
            }

            public void Release()
            {

            }



            public void Default()
            {
                Margin = new Thickness(10);
                Height = 50;
                Width = 200;
                Background = new SolidColorBrush(new Windows.UI.Color() { A = 0x55, B = 0x55, G = 0x55, R = 0x55 });
            }
        }

        //List<s_Data> _testitems = new List<s_Data>();
        public void ItemCollectionInit()
        {
            byte[] tbarr = new byte[4 * 99];
            new Random().NextBytes(tbarr);
            for (int i = 0; i < 99; i++)
            {
                DataItems.Add(new s_Data()
                {
                    index = i,
                    DA = tbarr[i * 4],
                    DB = tbarr[i * 4 + 1],
                    DG = tbarr[i * 4 + 2],
                    DR = tbarr[i * 4 + 3]
                });
            }
        }


        private void Load_data_Click(object sender, RoutedEventArgs e)
        {
            //ItemCollectionInit();
            ////_testitems

            //for (int i = 0; i < _ContentPanel.Children.Count; i++)
            //{
            //    if (_ContentPanel.Children[i] is Grid _grid)
            //    {
            //        if (_grid.Children[0] is iDataControl _iDataControl)
            //        {
            //            ActiveItem(_testitems[i], _iDataControl);
            //        }
            //    }
            //}


        }




        private void Push_block_Click(object sender, RoutedEventArgs e)
        {
            //Grid _target = _ContentPanel.Children[0] as Grid;
            //for (int i = 1; i < _ContentPanel.Children.Count; i++)
            //{
            //    if (_ContentPanel.Children[i] is Grid _grid)
            //    {
            //        if (_grid.RenderTransform is CompositeTransform _CompositeTransform)
            //        {
            //            if (_target.RenderTransform is CompositeTransform _CompositeTransform1)
            //            {
            //                if (_CompositeTransform.TranslateY < _CompositeTransform1.TranslateY)
            //                    _target = _grid;
            //            }
            //        }
            //    }
            //}





            //if (_target.RenderTransform is CompositeTransform _CompositeTransform2)
            //{
            //    _CompositeTransform2.TranslateY = _ContentPanel.ActualHeight;
            //    _ContentPanel.Height += _target.ActualHeight;
            //    _SetButtomWall(ButtomWall + _target.ActualHeight);
            //}

        }


        private void testFuncs()
        {


        }

        #endregion
    }

    #region 接口和基类定义
    /// <summary>
    /// 数据模型基类
    /// </summary>
    public abstract class VirtualModel
    {
        public abstract Type ControlType { get; }
    }


    /// <summary>
    /// 数据控件接口
    /// </summary>
    public interface iDataControl
    {
        /// <summary>
        /// 数据加载调用方法
        /// </summary>
        /// <param name="data"></param>
        void Load(VirtualModel data);

        /// <summary>
        /// 控件清空方法
        /// </summary>
        void Release();

        /// <summary>
        /// 控件加载默认样式
        /// </summary>
        void Default();
    }
    #endregion









}
