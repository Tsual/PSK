﻿using System;
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
    [Table(name: "StringSequence")]
    public class StringSequence
    {
        [Required, Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public string Data { get; set; }
    }

    [Table(name: "StringSequenceB")]
    public class StringSequenceB
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

    [Table(name: "Data")]
    public class Data
    {
        [Required, Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public string key { get; set; }

        public string value { get; set; }
    }
}