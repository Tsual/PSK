using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSK.Helper
{
    /// <summary>
    /// 通用绑定类
    /// </summary>
    /// <typeparam name="T1">需要绑定的 T1 类型</typeparam>
    /// <typeparam name="T2">需要绑定的 T2 类型</typeparam>
    public class GT_Binder<T1,T2>
    {
        /// <summary>
        /// 绑定元
        /// </summary>
        private class GT_Binder_Pac
        {
            private T1 _T1;
            private T2 _T2;
        }

        public class GT_Binder_NotFindException:Exception
        {
            public override string Message => "Data not found";
            T1 Data1 { get; set; }
            T2 Data2 { get; set; }
        }


        private bool T1Nullable = false;
        private bool T2Nullable = false;

        public GT_Binder(bool T1Nullable,bool T2Nullable)
        {
            this.T1Nullable = T1Nullable;
            this.T2Nullable = T2Nullable;
        }

        /// <summary>
        /// 绑定集合列表
        /// </summary>
        List<GT_Binder_Pac> _list = new List<GT_Binder_Pac>();

        private T2 Findt2(T1 obj)
        {
            
        }

        private T1 Findt1(T2 obj)
        {

        }






    }
}
