﻿using PSK.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using System.Collections.Specialized;
using Windows.Security.Cryptography.Core;

namespace PSK.UserComponent
{
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

                return new Helper.AESProvider(_iv, _key);
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
                        UserPwdVertifyFailEvent?.Invoke(this);
                        return null;
                    }
                }
                int uid = (from t in db.Users.ToList()
                           where PID == t.pid && pwd_hash_aes == t.pwd
                           select t).ToList().ElementAt(0).ID;
                UserVertifyEvent?.Invoke(this);
                return new CurrentUser((from t in db.Recordings.ToList()
                                        where t.uid == uid
                                        select t).ToList(), PID, PWD_hash, uid);
            }
        }


        public delegate void UserVertifyEventHandler(LoginUser user);
        public event UserVertifyEventHandler UserVertifyEvent;

        public delegate void UserPwdVertifyFailEventHandler(LoginUser user);
        public event UserPwdVertifyFailEventHandler UserPwdVertifyFailEvent;

        //public delegate void UserCreatePidExistEventHandler(LoginUser user);
        //public event UserCreatePidExistEventHandler UserCreatePidExistEvent;

        public enum UserNotFoundReceipt { Create, None }
        public delegate UserNotFoundReceipt UserNotFoundEventHandler(LoginUser user);
        public event UserNotFoundEventHandler UserNotFoundEvent;
    }
}
