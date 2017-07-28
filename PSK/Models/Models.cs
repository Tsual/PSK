using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PSK.UserComponent;
using Windows.Storage;
using Windows.Storage.Streams;
using System.Runtime.Serialization;
using System.IO;
using Windows.Security.Cryptography.Core;
using System.Collections.ObjectModel;

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
                    if (_old > 0) Core.Current.CurrentUser.Recordings[_old] = this;
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
                    if (_old > 0) Core.Current.CurrentUser.Recordings[_old] = this;
                }
            }
        }

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

    public class UI_Info_Str
    {
        public string str { get; set; }
    }

    public class UI_Info
    {


        public Info _Info { get; set; }
        public ObservableCollection<UI_Info_Str> Lines { get => _Lines; }
        private ObservableCollection<UI_Info_Str> _Lines = new ObservableCollection<UI_Info_Str>();
        private int _infoindex = 0;


        public UI_Info(Info info)
        {
            if (info != null)
                _Info = info;
            else
                throw new NullReferenceException();
            _infoindex = Core.Current.CurrentUser.Recordings.IndexOf(info);
            _deserialize();
            _Lines.CollectionChanged += _Lines_CollectionChanged;
        }

        public void _Lines_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            _serializeAsync();
        }

        private async void _serializeAsync()
        {
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    using (var ost = ms.AsOutputStream())
                    {
                        DataContractSerializer ser = new DataContractSerializer(typeof(ObservableCollection<UI_Info_Str>));
                        ser.WriteObject(ost.AsStreamForWrite(), _Lines);
                        await ost.FlushAsync();
                        var res = Encoding.UTF8.GetString(ms.ToArray());
                        Core.Current.CurrentUser.Recordings[_infoindex] = new Info() { DetailName = _Info.DetailName, Record = _Info.Record, Detail = res };
                    }


                }
            }
            finally { }





        }

        private void _deserialize()
        {
            if (_Info.Detail == "") return;
            try
            {
                using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(_Info.Detail)))
                {
                    using (var ist = ms.AsInputStream())
                    {
                        DataContractSerializer ser = new DataContractSerializer(typeof(ObservableCollection<UI_Info_Str>));
                        if (ser.ReadObject(ist.AsStreamForRead()) is ObservableCollection<UI_Info_Str> res)
                        {
                            var t = res;
                            _Lines = res;
                        }
                    }
                }
            }
            finally { }


        }
    }


    public class DataPac
    {
        public class dRow
        {
            public string key { get; set; }
            public string value { get; set; }
        }


        public dRow[] dRows { get; set; }
        public string token { get; set; }
        public string sertoken { get; set; }
        public string pid { get; set; }
        public string pwd_hash { get; set; }
        public string vertifystr { get; set; }

        public async Task SerializeAsync(StorageFile sf)
        {
            using (IRandomAccessStream rastream = await sf.OpenAsync(FileAccessMode.ReadWrite))
            {
                using (IOutputStream os = rastream.GetOutputStreamAt(0))
                {
                    DataContractSerializer ser = new DataContractSerializer(typeof(DataPac));
                    ser.WriteObject(os.AsStreamForWrite(), this);
                    await os.FlushAsync();
                }
            }
        }

        public static async Task<DataPac> DeserializeAsync(StorageFile sf)
        {
            using (IRandomAccessStream rastream = await sf.OpenAsync(FileAccessMode.ReadWrite))
            {
                using (IInputStream istraem = rastream.GetInputStreamAt(0))
                {
                    DataContractSerializer ser = new DataContractSerializer(typeof(DataPac));
                    var datpac = ser.ReadObject(istraem.AsStreamForRead()) as DataPac;
                    return datpac;
                }
            }
        }






    }

    public class DataPacManager
    {

        //useage
        //var spicker = new FileSavePicker();
        //spicker.FileTypeChoices.Add("XML", new List<String>() { ".xml" });
        //var sfs = await spicker.PickSaveFileAsync();
        //await DataPacManager.SerializeAsync(sfs, "test");
        public static async Task SerializeAsync(StorageFile sf, string serkey)
        {
            if (Core.Current.CurrentUser == null) throw new Exception();
            DataPac dat = new DataPac();
            await Task.Run(async () =>
            {
                //create ser aesobj
                string pid = Core.Current.CurrentUser.PID;
                dat.pid = pid;
                string str1 = pid + serkey;
                string str2 = serkey + pid;
                var hashobj = new Helper.HashProvider();
                string hstr1 = hashobj.Hash(str1);
                string hstr2 = hashobj.Hash(str2);
                string serpwd_hash = hstr1 + hstr2;
                Helper.AESProvider _AESobj;
                var ivhash = new Helper.HashProvider(HashAlgorithmNames.Md5);
                byte[] _iv = ivhash.Hashbytes(pid);
                //create token
                string ranstr = (new Helper.RandomGenerator()).getRandomString(20);
                dat.token = ranstr;
                string kstr1 = ranstr + serpwd_hash;
                string kstr2 = serpwd_hash + ranstr;

                var keyhash = new Helper.HashProvider();
                byte[] _key = new byte[128];
                byte[] btar = keyhash.Hashbytes(kstr1);
                Array.Copy(btar, 0, _key, 0, 64);
                btar = keyhash.Hashbytes(kstr2);
                Array.Copy(btar, 0, _key, 64, 64);
                _AESobj = new Helper.AESProvider(_iv, _key);
                //encrypt dats
                dat.pwd_hash = _AESobj.Encrypt(Core.Current.CurrentUser.PWD_hash);
                dat.sertoken = _AESobj.Encrypt(ranstr);
                dat.vertifystr = _AESobj.Encrypt(AssetsController.getLocalSequenceString(Core.Current.CurrentUser.UID));
                List<DataPac.dRow> arr = new List<DataPac.dRow>();
                using (APPDbContext db = new APPDbContext())
                {
                    foreach (var t in db.Recordings.ToList())
                    {
                        if (t.uid == Core.Current.CurrentUser.UID)
                        {
                            DataPac.dRow obj = new DataPac.dRow()
                            {
                                key = _AESobj.Encrypt(t.key),
                                value = _AESobj.Encrypt(t.value)
                            };
                            arr.Add(obj);
                        }
                    }
                }
                dat.dRows = arr.ToArray();
                await dat.SerializeAsync(sf);
            });
        }

        //useage
        //var picker = new FileOpenPicker();
        //picker.FileTypeFilter.Add(".xml");
        //    var sf = await picker.PickSingleFileAsync();
        //await DataPacManager.DeserializeAsync(sf, "test");@throw KeyVertifyFailException
        public static async Task DeserializeAsync(StorageFile sf, string serkey)
        {
            var dat = await DataPac.DeserializeAsync(sf);
            string pid = dat.pid;
            string token = dat.token;
            string desertoken = "";
            Helper.AESProvider _AESobj = null;
            await Task.Run(() =>
            {
                //create ser aesobj
                string str1 = pid + serkey;
                string str2 = serkey + pid;
                var hashobj = new Helper.HashProvider();
                string hstr1 = hashobj.Hash(str1);
                string hstr2 = hashobj.Hash(str2);
                string serpwd_hash = hstr1 + hstr2;
                var ivhash = new Helper.HashProvider(HashAlgorithmNames.Md5);
                byte[] _iv = ivhash.Hashbytes(pid);

                string kstr1 = token + serpwd_hash;
                string kstr2 = serpwd_hash + token;

                var keyhash = new Helper.HashProvider();
                byte[] _key = new byte[128];
                byte[] btar = keyhash.Hashbytes(kstr1);
                Array.Copy(btar, 0, _key, 0, 64);
                btar = keyhash.Hashbytes(kstr2);
                Array.Copy(btar, 0, _key, 64, 64);
                _AESobj = new Helper.AESProvider(_iv, _key);
                desertoken = _AESobj.Decrypt(dat.sertoken);
            });
            if (desertoken != token) throw new KeyVertifyFailException() { _DataPac = dat };
            await Task.Run(() =>            //InvalidOperationException
            {
                using (APPDbContext db = new APPDbContext())
                {
                    //search user
                    User user = null;
                    try
                    {
                        user = db.Users.Single(b => b.pid == pid);
                    }
                    catch (InvalidOperationException) { }

                    if (user == null)
                    {
                        string pwd_hash_aes = AssetsController.EncryptwithAppaesobj(_AESobj.Decrypt(dat.pwd_hash));
                        User dbuser = new User()
                        {
                            pid = pid,
                            pwd = pwd_hash_aes
                        };
                        db.Entry(dbuser).State = Microsoft.EntityFrameworkCore.EntityState.Added;
                        db.SaveChanges();
                        user = db.Users.Single(b => b.pid == pid); ;
                    }
                    //set user sa
                    AssetsController.getLocalSequenceString(user.ID);
                    var saobj = db.SA.Single(t => t.ID == user.ID);
                    //saobj.Data = dat.vertifystr;
                    saobj.Data = _AESobj.Decrypt(dat.vertifystr);
                    db.Entry(saobj).State = Microsoft.EntityFrameworkCore.EntityState.Modified;

                    foreach (var t in dat.dRows)
                    {
                        Recording obj = new Recording()
                        {
                            uid = user.ID,
                            key = _AESobj.Decrypt(t.key),
                            value = _AESobj.Decrypt(t.value)
                        };
                        db.Entry(obj).State = Microsoft.EntityFrameworkCore.EntityState.Added;
                    }
                    db.SaveChanges();
                }
            });
        }

        public class KeyVertifyFailException : Exception
        {
            public override string Message => "decode sertoken cannot match token";
            public DataPac _DataPac { get; set; }
        }


    }

}
