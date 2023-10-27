using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentResults;

namespace Cryptosuite.Core.Interfaces
{
    public interface IProofPurpose
    {
        public Result Validate(Proof proof);
        public Proof Update(Proof proof);
        public bool Match(Proof proof);
    }
}
