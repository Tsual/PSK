using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;

namespace PSK.Helper
{
    public class AESProvider
    {
        byte[] _iv = new byte[16];
        byte[] _key = new byte[128];
        public AESProvider(byte[] iv, byte[] key)
        {
            if (iv.Length == 16 && key.Length == 128)
            {
                Array.Copy(iv, _iv, 16);
                Array.Copy(key, _key, 128);
            }
            else
            {
                throw new Exception("iv or key length error");
            }
        }

        public string Encrypt(string metaStr)
        {
            #region metaStr_buffer Add0
            byte[] metaStr_byte = Encoding.UTF8.GetBytes(metaStr);
            int metaStr_byte_count = metaStr_byte.Count();
            int metaStr_byte_f_count = (metaStr_byte_count / 16 + 1) * 16;
            byte[] metaStr_byte_f = new byte[metaStr_byte_f_count];
            for (int i = 0; i < metaStr_byte_f_count; i++)
            {
                if (i < metaStr_byte_count) metaStr_byte_f[i] = metaStr_byte[i];
                else metaStr_byte_f[i] = 0;
            }
            var metaStr_buffer = CryptographicBuffer.CreateFromByteArray(metaStr_byte_f);
            #endregion

            #region _Key
            var _SymmetricKeyAlgorithmProvider = SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithmNames.AesCbc);
            var _Key = _SymmetricKeyAlgorithmProvider.CreateSymmetricKey(CryptographicBuffer.CreateFromByteArray(this._key));
            #endregion

            #region IV_buffer
            var IV_buffer = CryptographicBuffer.CreateFromByteArray(_iv);
            #endregion

            IBuffer _result_buffer = CryptographicEngine.Encrypt(_Key, metaStr_buffer, IV_buffer);
            return CryptographicBuffer.EncodeToHexString(_result_buffer);
        }
        public string Decrypt(string metaStr)
        {

            #region metaStr_buffer
            var metaStr_buffer = CryptographicBuffer.DecodeFromHexString(metaStr);
            #endregion

            #region _Key
            var _SymmetricKeyAlgorithmProvider = SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithmNames.AesCbc);
            var _Key = _SymmetricKeyAlgorithmProvider.CreateSymmetricKey(CryptographicBuffer.CreateFromByteArray(this._key));
            #endregion

            #region IV_buffer
            var IV_buffer = CryptographicBuffer.CreateFromByteArray(_iv);
            #endregion

            IBuffer _result_buffer = CryptographicEngine.Decrypt(_Key, metaStr_buffer, IV_buffer);
            byte[] dat = new byte[256];
            CryptographicBuffer.CopyToByteArray(_result_buffer, out dat);

            #region rebuild string
            List<byte> list_byte = new List<byte>();
            for (int i = 0; i < dat.Count(); i++)
            {
                if (dat[i] != 0)
                    list_byte.Add(dat[i]);
            }
            #endregion
            return Encoding.UTF8.GetString(list_byte.ToArray());
        }
    }
}
