using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptonite.ECDH.Core
{
    internal static class Salsa20
    {
        internal static Span<byte> HSalsa20(Span<byte> key)
        {
            Span<byte> result = new Span<byte>(new byte[32]);
            key.CopyTo(result);

            var state = LoadLittleEndian(result);
            SalsaCore.Salsa20(ref state, 10);
            StoreLittleEndian(result, ref state);

            return result;
        }

        private const uint SalsaConst0 = 0x61707865;
        private const uint SalsaConst1 = 0x3320646e;
        private const uint SalsaConst2 = 0x79622d32;
        private const uint SalsaConst3 = 0x6b206574;

        private unsafe static Vec16<uint> LoadLittleEndian(Span<byte> result)
        {
            Span<byte> nonce = stackalloc byte[16];
            return new Vec16<uint>
            {
                X0 = SalsaConst0,
                X1 = LoadLittleEndian32(result, 0),
                X2 = LoadLittleEndian32(result, 4),
                X3 = LoadLittleEndian32(result, 8),
                X4 = LoadLittleEndian32(result, 12),
                X5 = SalsaConst1,
                X6 = LoadLittleEndian32(nonce, 0),
                X7 = LoadLittleEndian32(nonce, 4),
                X8 = LoadLittleEndian32(nonce, 8),
                X9 = LoadLittleEndian32(nonce, 12),
                X10 = SalsaConst2,
                X11 = LoadLittleEndian32(result, 16),
                X12 = LoadLittleEndian32(result, 20),
                X13 = LoadLittleEndian32(result, 24),
                X14 = LoadLittleEndian32(result, 28),
                X15 = SalsaConst3
            };
        }

        private static uint LoadLittleEndian32(Span<byte> buf, int offset = 0)
            => buf[offset + 0]
               | (uint)buf[offset + 1] << 8
               | (uint)buf[offset + 2] << 16
               | (uint)buf[offset + 3] << 24;

        private static void StoreLittleEndian(Span<byte> result, ref Vec16<uint> state)
        {
            StoreLittleEndian32(result, state.X0, 0);
            StoreLittleEndian32(result, state.X5, 4);
            StoreLittleEndian32(result, state.X10, 8);
            StoreLittleEndian32(result, state.X15, 12);
            StoreLittleEndian32(result, state.X6, 16);
            StoreLittleEndian32(result, state.X7, 20);
            StoreLittleEndian32(result, state.X8, 24);
            StoreLittleEndian32(result, state.X9, 28);
        }

        private static void StoreLittleEndian32(Span<byte> buf, uint value, int offset = 0)
        {
            buf[offset + 0] = unchecked((byte)value);
            buf[offset + 1] = unchecked((byte)(value >> 8));
            buf[offset + 2] = unchecked((byte)(value >> 16));
            buf[offset + 3] = unchecked((byte)(value >> 24));
        }
    }

    internal static class SalsaCore
    {
        internal static void Salsa20(ref Vec16<uint> state, int doubleRounds)
        {
            unchecked
            {
                for (var i = 0; i < doubleRounds; i++)
                {
                    #region RowRound
                    // row 0
                    state.X4 ^= state.X0 + state.X12 << 7 | state.X0 + state.X12 >> 32 - 7;
                    state.X8 ^= state.X4 + state.X0 << 9 | state.X4 + state.X0 >> 32 - 9;
                    state.X12 ^= state.X8 + state.X4 << 13 | state.X8 + state.X4 >> 32 - 13;
                    state.X0 ^= state.X12 + state.X8 << 18 | state.X12 + state.X8 >> 32 - 18;

                    // row 1
                    state.X9 ^= state.X5 + state.X1 << 7 | state.X5 + state.X1 >> 32 - 7;
                    state.X13 ^= state.X9 + state.X5 << 9 | state.X9 + state.X5 >> 32 - 9;
                    state.X1 ^= state.X13 + state.X9 << 13 | state.X13 + state.X9 >> 32 - 13;
                    state.X5 ^= state.X1 + state.X13 << 18 | state.X1 + state.X13 >> 32 - 18;

                    // row 2
                    state.X14 ^= state.X10 + state.X6 << 7 | state.X10 + state.X6 >> 32 - 7;
                    state.X2 ^= state.X14 + state.X10 << 9 | state.X14 + state.X10 >> 32 - 9;
                    state.X6 ^= state.X2 + state.X14 << 13 | state.X2 + state.X14 >> 32 - 13;
                    state.X10 ^= state.X6 + state.X2 << 18 | state.X6 + state.X2 >> 32 - 18;

                    // row 3
                    state.X3 ^= state.X15 + state.X11 << 7 | state.X15 + state.X11 >> 32 - 7;
                    state.X7 ^= state.X3 + state.X15 << 9 | state.X3 + state.X15 >> 32 - 9;
                    state.X11 ^= state.X7 + state.X3 << 13 | state.X7 + state.X3 >> 32 - 13;
                    state.X15 ^= state.X11 + state.X7 << 18 | state.X11 + state.X7 >> 32 - 18;
                    #endregion

                    #region ColumnRound
                    // column 0
                    state.X1 ^= state.X0 + state.X3 << 7 | state.X0 + state.X3 >> 32 - 7;
                    state.X2 ^= state.X1 + state.X0 << 9 | state.X1 + state.X0 >> 32 - 9;
                    state.X3 ^= state.X2 + state.X1 << 13 | state.X2 + state.X1 >> 32 - 13;
                    state.X0 ^= state.X3 + state.X2 << 18 | state.X3 + state.X2 >> 32 - 18;

                    // column 1
                    state.X6 ^= state.X5 + state.X4 << 7 | state.X5 + state.X4 >> 32 - 7;
                    state.X7 ^= state.X6 + state.X5 << 9 | state.X6 + state.X5 >> 32 - 9;
                    state.X4 ^= state.X7 + state.X6 << 13 | state.X7 + state.X6 >> 32 - 13;
                    state.X5 ^= state.X4 + state.X7 << 18 | state.X4 + state.X7 >> 32 - 18;

                    // column 2
                    state.X11 ^= state.X10 + state.X9 << 7 | state.X10 + state.X9 >> 32 - 7;
                    state.X8 ^= state.X11 + state.X10 << 9 | state.X11 + state.X10 >> 32 - 9;
                    state.X9 ^= state.X8 + state.X11 << 13 | state.X8 + state.X11 >> 32 - 13;
                    state.X10 ^= state.X9 + state.X8 << 18 | state.X9 + state.X8 >> 32 - 18;

                    // column 3
                    state.X12 ^= state.X15 + state.X14 << 7 | state.X15 + state.X14 >> 32 - 7;
                    state.X13 ^= state.X12 + state.X15 << 9 | state.X12 + state.X15 >> 32 - 9;
                    state.X14 ^= state.X13 + state.X12 << 13 | state.X13 + state.X12 >> 32 - 13;
                    state.X15 ^= state.X14 + state.X13 << 18 | state.X14 + state.X13 >> 32 - 18;
                    #endregion
                }
            }
        }
    }
}
