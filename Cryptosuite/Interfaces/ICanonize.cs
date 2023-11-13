using JsonLdExtensions;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptosuite.Core.Interfaces
{
    public interface ICanonize
    {
        string Canonize(JToken input, JsonLdNormalizerOptions options);
    }
}
