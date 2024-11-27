﻿namespace Cryptosuite.Core.Interfaces
{
    public interface ICryptosuite
    {
        string RequiredAlgorithm { get; }
        string Name { get; }
        static abstract string TypeName { get; }
    }
}