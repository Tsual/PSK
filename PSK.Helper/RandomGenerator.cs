using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSK.Helper
{
    public class RandomGenerator
    {
        static char getchar(int i)
        {
            if (i < 0) return ' ';
            else if (i < 10) return Convert.ToChar(i + 48);
            else if (i < 36) return Convert.ToChar(i + 55);
            else if (i < 62) return Convert.ToChar(i + 61);
            else return ' ';
        }


        Random ran;
        public RandomGenerator()
        {
            ran = new Random();
        }

        public string getRandomString(int len)
        {
            string res = "";
            for (int i = 0; i < len; i++) res += getchar(ran.Next(62));
            return res;
        }

        public int Next(int i)
        {
            return ran.Next(i);
        }

    }
}
