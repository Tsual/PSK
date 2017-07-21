using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public string Title { get; set; }
        public string Detail { get; set; }

        public Recording Encode(CurrentUser user)
        {
            Record = new Recording();
            Record.key = user.Encode(Title);
            Record.value = user.Encode(Detail);
            Record.uid = user.UID;
            return Record;
        }

        public Info()
        {

        }

        public Info(Recording record,CurrentUser user)
        {
            this.Record = record;
            this.Title = user.Decode(record.key);
            this.Detail = user.Decode(record.value);
        }
    }
}
