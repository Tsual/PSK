using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PSK.UserComponent;

/* Models*/
/*  
 [Table(name: "testtable")]
    public class testmodel
    {
        [Required, Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public string Info { get; set; }

        public DateTime Time { get; set; }
    }
    */

namespace PSK.Models
{
    [Table(name: "StringSequenceA")]
    public class StringSequenceObjA
    {
        [Required, Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public string Data { get; set; }
    }

    [Table(name: "StringSequenceB")]
    public class StringSequenceObjB
    {
        [Required, Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public string Data { get; set; }
    }

    [Table(name: "User")]
    public class User
    {
        [Required, Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public string pid { get; set; }

        public string pwd { get; set; }
    }

    [Table(name: "Recording")]
    public class Recording
    {
        [Required, Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public string key { get; set; }

        public string value { get; set; }

        public int uid { get; set; }

    }

    public class Info
    {
        public Recording Record { get; set; }

        string _DetailName = "";
        public string DetailName
        {
            get { return _DetailName; }
            set
            {
                _DetailName = value;
                if (Core.Current.CurrentUser != null)
                {
                    var _old = Core.Current.CurrentUser.Recordings.IndexOf(this);
                    Core.Current.CurrentUser.Recordings[_old] = this;
                }
            }
        }

        string _Detail = "";
        public string Detail
        {
            get { return _Detail; }
            set
            {
                _Detail = value;
                if (Core.Current.CurrentUser != null)
                {
                    var _old = Core.Current.CurrentUser.Recordings.IndexOf(this);
                    Core.Current.CurrentUser.Recordings[_old] = this;
                }
            }
        }

        public bool Switchbool
        {
            get { return _Switchbool; }
            set { _Switchbool = value; SwitchChangedEvent?.Invoke(this); }
        }
        private bool _Switchbool = false;

        public delegate void SwitchChangedEventHandler(Info f);
        public event SwitchChangedEventHandler SwitchChangedEvent;
        public bool isSwitchChangedEventNull { get { return SwitchChangedEvent == null ? true : false; } }

        public Recording Encode(CurrentUser user)
        {
            Record = new Recording();
            Record.key = user.Encode(DetailName);
            Record.value = user.Encode(Detail);
            Record.uid = user.UID;
            return Record;
        }

        public Recording Modify(CurrentUser user)
        {
            Record.key = user.Encode(DetailName);
            Record.value = user.Encode(Detail);
            Record.uid = user.UID;
            return Record;
        }

        public Info()
        {

        }

        public Info(Recording record, CurrentUser user)
        {
            this.Record = record;
            this.DetailName = user.Decode(record.key);
            this.Detail = user.Decode(record.value);
        }
    }
}
