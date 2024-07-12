using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptosuite.Core.Util
{
    public static class BaseConvert
    {
        public static string ToBase64UrlNoPadding(byte[] data)
        {
            return Convert.ToBase64String(data).Replace('+', '-').Replace('/', '_').Remove('=');
        }

        public static byte[] FromBase64UrlNoPadding(string data)
        {
            data = data.Replace('-', '+').Replace('_', '/');
            int padding = data.Length % 4;
            if (padding > 0)
            {
                data += new string('=', 4 - padding);
            }
            return Convert.FromBase64String(data);
        }
    }
}
