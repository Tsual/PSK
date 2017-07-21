using PSK.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using System.Collections.Specialized;
using Windows.Security.Cryptography.Core;

namespace PSK
{
    public class AssetsController
    {
        static string STR_FS = "firstinstall";
        static string STR_APP_IV = "appiv";
        static string STR_APP_KEY = "appkey";

        private static byte[] _appiv;
        private static byte[] _appkey;

        static byte[] decodeappiv(string ivstr)
        {
            var slist = ivstr.Split('|');
            if (slist.Length != 16) throw new Exception("iv dec error");
            byte[] res = new byte[16];
            for (int i = 0; i < 16; i++)
            {
                res[i] = Convert.ToByte(slist[i]);
            }
            return res;

        }
        static string encodeappiv(byte[] iv)
        {
            if (iv == null || iv.Length != 16) throw new Exception("iv error");
            string res = "" + iv[0];
            for (int i = 1; i < 16; i++)
            {
                res += "|" + iv[i];
            }
            return res;
        }
        static byte[] createappiv()
        {
            byte[] res = new byte[16];
            new Random().NextBytes(res);
            return res;
        }

        static byte[] decodeappkey(string keystr)
        {
            var slist = keystr.Split('|');
            if (slist.Length != 128) throw new Exception("iv dec error");
            byte[] res = new byte[128];
            for (int i = 0; i < 128; i++)
            {
                res[i] = Convert.ToByte(slist[i]);
            }
            return res;
        }
        static string encodeappkey(byte[] key)
        {
            if (key == null || key.Length != 128) throw new Exception("key error");
            string res = "" + key[0];
            for (int i = 1; i < 128; i++)
            {
                res += "|" + key[i];
            }
            return res;
        }
        static byte[] createappkey()
        {
            byte[] res = new byte[128];
            new Random().NextBytes(res);
            return res;
        }

        public static bool Reset()
        {
            try
            {
                ApplicationDataContainer sets = ApplicationData.Current.LocalSettings;
                sets.Values.Remove(STR_FS);
                sets.Values.Remove(STR_APP_IV);
                sets.Values.Remove(STR_APP_KEY);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public static string EncryptwithAppFI(string metaStr)
        {
            var aesobj = new Helper.AESProvider(_appiv, _appkey);
            return aesobj.Encrypt(metaStr);
        }

        public static string getLocalSequenceString(int id)
        {
            using (APPDbContext db = new APPDbContext())
            {
                var res = db.SA.Find(id);
                if (res != null)
                    return res.Data;
                else
                {
                    var ncount = db.SA.Count();
                    var rans = new Helper.RandomGenerator();
                    for (int i = 0; i < id - ncount; i++)
                    {
                        db.SA.Add(new StringSequenceObjA() { Data = rans.getRandomString(20) });
                    }
                    db.SaveChanges();
                    res = db.SA.Find(id);
                    if (res == null) throw new Exception("sa id miss");
                    return res.Data;
                }
            }
        }

        static AssetsController()
        {
            //init appiv appkey
            ApplicationDataContainer sets = ApplicationData.Current.LocalSettings;
            if (sets.Values.Keys.Contains(STR_FS))
            {
                _appiv = AssetsController.decodeappiv((string)sets.Values[STR_APP_IV]);
                _appkey = AssetsController.decodeappkey((string)sets.Values[STR_APP_KEY]);
            }
            else
            {
                sets.Values[STR_FS] = "false";
                _appiv = createappiv();
                sets.Values[STR_APP_IV] = encodeappiv(_appiv);
                _appkey = createappkey();
                sets.Values[STR_APP_KEY] = encodeappkey(_appkey);
            }







        }

        public static void test()
        {

            //delete all records in users
            //using (APPDbContext db = new APPDbContext())
            //{
            //    db.Database.EnsureCreated();

            //    var list = (from t in db.Users.ToList()
            //                select t).ToList();
            //    foreach (var t in list)
            //    {
            //        db.Entry(t).State = Microsoft.EntityFrameworkCore.EntityState.Deleted;

            //    }
            //    db.SaveChanges();
            //}

            using (APPDbContext db = new APPDbContext())
            {
                db.Database.EnsureCreated();

                var list = (from t in db.Recordings.ToList()
                            select t).ToList();
                foreach (var t in list)
                {
                    db.Entry(t).State = Microsoft.EntityFrameworkCore.EntityState.Deleted;

                }
                db.SaveChanges();
            }



            var user = LoginUser.CreateObj("test", "test");
            user.UserNotFoundEvent += (obj) => { return LoginUser.UserNotFoundReceipt.Create; };
            var cuser = user.TryLogin();
            var item = new Info() { Title = "ffff", Detail = "detail" };
            cuser.Recordings.Add(item);
            cuser.Recordings.Remove(item);














            int a = 0;
        }

        #region api



        #endregion




    }

    public class CurrentUser
    {
        internal CurrentUser(List<Recording> list, string PID, string PWD_hash, int UID)
        {

            this.PID = PID;
            this.PWD_hash = PWD_hash;
            this.UID = UID;
            foreach (var t in list)
                recordings.Add(new Info(t, this));
            recordings.CollectionChanged += Recordings_CollectionChanged;
        }

        private void Recordings_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            using (APPDbContext db = new APPDbContext())
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        foreach (var t in e.NewItems)
                        {
                            db.Entry(((Info)t).Encode(this)).State =
                                Microsoft.EntityFrameworkCore.EntityState.Added;
                        }
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        foreach (var t in e.OldItems)
                        {
                            db.Entry(((Info)t).Record).State =
                                Microsoft.EntityFrameworkCore.EntityState.Deleted;
                        }
                        break;
                    case NotifyCollectionChangedAction.Replace:
                        foreach (var t in e.OldItems)
                        {
                            db.Entry(((Info)t).Record).State =
                                Microsoft.EntityFrameworkCore.EntityState.Deleted;
                        }
                        foreach (var t in e.NewItems)
                        {
                            db.Entry(((Info)t).Encode(this)).State =
                                Microsoft.EntityFrameworkCore.EntityState.Added;
                        }
                        break;
                    case NotifyCollectionChangedAction.Reset:
                        foreach (var t in e.OldItems)
                        {
                            db.Entry(((Info)t).Record).State =
                                Microsoft.EntityFrameworkCore.EntityState.Deleted;
                        }
                        foreach (var t in e.NewItems)
                        {
                            db.Entry(((Info)t).Encode(this)).State =
                                Microsoft.EntityFrameworkCore.EntityState.Added;
                        }
                        break;
                }
                db.SaveChanges();
            }
        }

        public string PID { get; private set; }
        public string PWD_hash { get; private set; }
        public int UID { get; private set; }
        private Helper.AESProvider AESobj
        {
            get
            {
                var ivhash = new Helper.HashProvider(HashAlgorithmNames.Md5);
                byte[] _iv = ivhash.Hashbytes(PID);

                string ranstr = AssetsController.getLocalSequenceString(UID);
                string kstr1 = ranstr + PWD_hash;
                string kstr2 = PWD_hash + ranstr;

                var keyhash = new Helper.HashProvider();
                byte[] _key = new byte[128];
                byte[] btar = keyhash.Hashbytes(kstr1);
                Array.Copy(btar, 0, _key, 0, 64);
                btar = keyhash.Hashbytes(kstr2);
                Array.Copy(btar, 0, _key, 64, 64);

                return  new Helper.AESProvider(_iv, _key);
            }
        }

        public ObservableCollection<Info> Recordings { get => recordings; }
        private ObservableCollection<Info> recordings = new ObservableCollection<Info>();

        public string Decode(string metaStr)
        {
            return AESobj.Decrypt(metaStr);
        }
        public string Encode(string metaStr)
        {
            return AESobj.Encrypt(metaStr);
        }

    }

    public class LoginUser
    {
        public string PID { get; set; }
        public string PWD_hash { get; set; }

        protected LoginUser()
        {

        }

        public static LoginUser CreateObj(string pid, string pwd)
        {
            var user = new LoginUser();
            user.PID = pid;

            //加盐方式 str1=pid+pwd str2=pwd+pid
            string str1 = pid + pwd;
            string str2 = pwd + pid;
            var hashobj = new Helper.HashProvider();
            string hstr1 = hashobj.Hash(str1);
            string hstr2 = hashobj.Hash(str2);
            user.PWD_hash = hstr1 + hstr2;

            return user;
        }

        public CurrentUser TryLogin()
        {
            if (UserNotFoundEvent == null) UserNotFoundEvent += (obj) => { return UserNotFoundReceipt.None; };
            if (UserPwdVertifyFailEvent == null) UserPwdVertifyFailEvent += (obj) => { };

            using (APPDbContext db = new APPDbContext())
            {
                var pwd_hash_aes = AssetsController.EncryptwithAppFI(PWD_hash);
                var iuserlist = from t in db.Users.ToList()
                                where t.pid == PID
                                select t;
                var userlist = iuserlist.ToList();
                if (userlist.Count == 0)
                {
                    switch (UserNotFoundEvent(this))
                    {
                        case UserNotFoundReceipt.Create:
                            User dbuser = new User()
                            {
                                pid = PID,
                                pwd = pwd_hash_aes
                            };
                            db.Entry(dbuser).State = Microsoft.EntityFrameworkCore.EntityState.Added;
                            db.SaveChanges();
                            break;
                        case UserNotFoundReceipt.None:
                            return null;
                    }
                }
                else
                {
                    var vertifyuser = userlist[0];
                    if (vertifyuser.pid != this.PID || vertifyuser.pwd != pwd_hash_aes)
                    {
                        UserPwdVertifyFailEvent(this);
                        return null;
                    }
                }
                int uid = (from t in db.Users.ToList()
                           where PID == t.pid && pwd_hash_aes == t.pwd
                           select t).ToList().ElementAt(0).ID;
                return new CurrentUser((from t in db.Recordings.ToList()
                                        where t.uid == uid
                                        select t).ToList(), PID, PWD_hash, uid);
            }
        }



        public delegate void UserPwdVertifyFailEventHandler(LoginUser user);
        public event UserPwdVertifyFailEventHandler UserPwdVertifyFailEvent;

        public enum UserNotFoundReceipt { Create, None }
        public delegate UserNotFoundReceipt UserNotFoundEventHandler(LoginUser user);
        public event UserNotFoundEventHandler UserNotFoundEvent;
    }

}
