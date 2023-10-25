using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptosuite
{
    public interface ICanonize
    {
        string Canonize(string input, object options);
    }
}
