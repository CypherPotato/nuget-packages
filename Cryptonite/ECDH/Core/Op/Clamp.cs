using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Cryptonite.ECDH.Core.Op
{
    internal static class ClampOperation
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void Clamp(Span<byte> s, int offset = 0)
        {
            s[offset + 0] &= 248;
            s[offset + 31] &= 127;
            s[offset + 31] |= 64;
        }
    }
}
