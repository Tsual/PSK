using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSK
{
    public class test_model : IVisualModel
    {
        public string Str { get; set; }

        public double VerValue { get; private set; }

        public double HorValue { get; private set; }
    }

    public interface IVisualCollectionItem<T>
    {
        T Obj { get; set; }
        void SetObj();
        void Init();
    }

    public interface IVisualCollecction<T>
    {
        IEnumerable<T> ItemsCollection { get; set; }
    }

    public interface IVisualModel
    {
        double VerValue { get; }
        double HorValue { get; }
    }
}
