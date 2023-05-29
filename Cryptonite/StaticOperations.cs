using Cryptonite.ECDH;
using Cryptonite.Scrypt.Utility;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace Cryptonite
{
    public static class StaticOperations
    {
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public static bool TimingSafeEqual(Span<byte> a, Span<byte> b)
        {
            bool result = true;
            if (a.Length != b.Length) return false;
            for (int i = 0; i < a.Length; i++)
            {
                result &= a[i] == b[i];
            }
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Span<byte> Range(Span<byte> a, int index, int length)
        {
            return a[index..(length + index)];
        }

        public static Span<byte> PadZeros(Span<byte> arr, int length, bool padFromLeft = false)
        {
            int inputLength = arr.Length;

            if (inputLength > length) throw new InvalidOperationException("The input array length must be lower or equal than the padding length.");
            if (length < 0) throw new InvalidOperationException("Length must be non negative.");
            if (inputLength == length) return arr;

            Span<byte> padObj = new Span<byte>(new byte[length]);
            if (padFromLeft)
            {
                Copy(arr, ref padObj, length - arr.Length);
            }
            else
            {
                Copy(arr, ref padObj, 0);
            }

            return padObj;
        }

        public static void Copy(Span<byte> from, ref Span<byte> to, int index)
        {
            for (int i = 0; i < from.Length; i++)
            {
                to[i + index] = from[i];
            }
        }

        public static Span<byte> XorGate(Span<byte> a, Span<byte> b)
        {
            if (a.Length != b.Length) throw new Exception("The arrays must be the same size.");

            Span<byte> outputKey = new Span<byte>(new byte[a.Length]);
            for (int i = 0; i < a.Length; i++)
            {
                outputKey[i] = (byte)(a[i] ^ b[i]);
            }

            return outputKey;
        }

        public static void RefXorGate(ref Span<byte> a, Span<byte> b)
        {
            if (a.Length != b.Length) throw new Exception("The arrays must be the same size.");
            for (int i = 0; i < a.Length; i++)
            {
                a[i] ^= b[i];
            }
        }

        public static void RefRandomSecureBytes(ref Span<byte> buffer)
        {
            RandomNumberGenerator.Create().GetBytes(buffer);
        }

        public static Span<byte> RandomSecureBytes(int length)
        {
            var bytes = new byte[length];
            RandomNumberGenerator.Create().GetBytes(bytes);
            return new Span<byte>(bytes);
        }

        public static Span<byte> ScryptDerive(Span<byte> key, int length, ScryptDeriveParameters parameters)
        {
            ArgumentNullException.ThrowIfNull(nameof(parameters.Salt));

            byte[] dkey = SCrypt.ComputeDerivedKey(
                key.ToArray(),
                parameters.Salt,
                parameters.MemoryCost,
                parameters.BlockSize,
                parameters.Pararellism,
                parameters.MaxThreads == -1 ? null : parameters.MaxThreads,
                length
            );
            return new Span<byte>(dkey);
        }

        public static Span<byte> Pbkdf2Derive(Span<byte> key, int length, PBKDF2Parameters parameters)
        {
            ArgumentNullException.ThrowIfNull(nameof(parameters.Salt));

            return new Span<byte>(Rfc2898DeriveBytes.Pbkdf2(
                key,
                parameters.Salt,
                parameters.Iterations,
                parameters.HashAlgorithm,
                length
            ));
        }
    }

    public class ScryptDeriveParameters
    {
        public byte[]? Salt { get; set; } = null;
        public int MemoryCost { get; set; } = 512;
        public int BlockSize { get; set; } = 8;
        public int Pararellism { get; set; } = 1;
        public int MaxThreads { get; set; } = -1;
    }

    public class PBKDF2Parameters
    {
        public byte[]? Salt { get; set; }
        public int Iterations { get; set; }
        public HashAlgorithmName HashAlgorithm { get; set; }

        public PBKDF2Parameters()
        {
            this.HashAlgorithm = HashAlgorithmName.SHA256;
            this.Iterations = 10000;
        }
    }
}