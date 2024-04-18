using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI_Sd_Primitives.Interfaces
{
    public interface ILabelMapFactoryFunction
    {
        Dictionary<string, string> CreateLabelMap(Dictionary<string, string> canonicalIdMap);
    }
}
