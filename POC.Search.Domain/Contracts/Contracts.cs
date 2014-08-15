using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace POC.Search.Domain.Contracts
{
    [ComVisible(true)]
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public struct unit
    {
        public static readonly unit it = default(unit);
    }
    [ComVisible(true)]
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public struct brandLookUpunit
    {
        public static readonly unit it = default(unit);
    }

    public enum AddOrUpdateHint
    {
        ProbablyExists,
        ProbablyDoesNotExist
    }
}
