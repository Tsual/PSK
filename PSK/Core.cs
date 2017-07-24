using PSK.Models;
using PSK.UserComponent;
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

        private AssetsController()
        {

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

            //using (APPDbContext db = new APPDbContext())
            //{
            //    db.Database.EnsureCreated();

            //    var list = (from t in db.Recordings.ToList()
            //                select t).ToList();
            //    foreach (var t in list)
            //    {
            //        db.Entry(t).State = Microsoft.EntityFrameworkCore.EntityState.Deleted;

            //    }
            //    db.SaveChanges();
            //}



            //var user = LoginUser.CreateObj("test", "fdfadfadsf");
            //user.UserNotFoundEvent += (obj) => { return LoginUser.UserNotFoundReceipt.Create; };
            //var cuser = user.TryLogin();
            //var item = new Info() { Title = "ffff", Detail = "detail" };
            //cuser.Recordings.Add(item);
            //cuser.Recordings.Remove(item);














            //int a = 0;
        }

        #region api



        #endregion




    }

    public class Core
    {
        public static CurrentUser CU { get; private set; }
        public static LoginUser LU { get; private set; }

        static Core()
        {

        }
    }

}

