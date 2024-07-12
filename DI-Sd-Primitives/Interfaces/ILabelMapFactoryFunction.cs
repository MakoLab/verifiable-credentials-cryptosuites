using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI_Sd_Primitives.Interfaces
{
    public interface ILabelMapFactoryFunction
    {
        IDictionary<string, string> CreateLabelMap(IDictionary<string, string> canonicalIdMap);
    }
}
