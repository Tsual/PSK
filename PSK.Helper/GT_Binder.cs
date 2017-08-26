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
    /// <typeparam name="T1">主键</typeparam>
    /// <typeparam name="T2">值 可为空 唯一约束</typeparam>
    public class GT_Binder<T1,T2>
    {
        Dictionary<T1, T2> dic = new Dictionary<T1, T2>();

        public T2 this[T1 obj]
        {
            get
            {
                return dic[obj];
            }
            set
            {
                


            }
        }



    }
}
