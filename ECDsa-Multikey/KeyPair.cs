using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ECDsa_Multikey
{
    internal class KeyPair
    {
        public string? Id { get; set; }
        public string? Controller { get; set; }
        public ECDsa? Keys { get; set; }
    }
}
