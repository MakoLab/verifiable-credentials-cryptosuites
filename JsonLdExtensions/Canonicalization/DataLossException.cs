using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonLdExtensions.Canonicalization
{
    public class DataLossException : ArgumentException
    {
        public DataLossException(string message) : base(message)
        {
        }
    }
}
