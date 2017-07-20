using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;

namespace PSK.Helper
{
    public class HashProvider
    {
        HashAlgorithmProvider AlgProv;
        CryptographicHash hashobj;



        public HashProvider()
        {
            AlgProv = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Sha512);
            hashobj = AlgProv.CreateHash();
        }
        public HashProvider(string name)
        {
            AlgProv = HashAlgorithmProvider.OpenAlgorithm(name);
            hashobj = AlgProv.CreateHash();
        }


        public string Hash(string str)
        {
            var _buffer = CryptographicBuffer.ConvertStringToBinary(str, BinaryStringEncoding.Utf8);
            hashobj.Append(_buffer);
            var _resbuffer = hashobj.GetValueAndReset();
            return CryptographicBuffer.EncodeToBase64String(_resbuffer);
        }

        public byte[] Hashbytes(string str)
        {
            var _buffer = CryptographicBuffer.ConvertStringToBinary(str, BinaryStringEncoding.Utf8);
            hashobj.Append(_buffer);
            var _resbuffer = hashobj.GetValueAndReset();
            byte[] res;
            CryptographicBuffer.CopyToByteArray(_resbuffer,out res);
            return res;
        }



    }
}
