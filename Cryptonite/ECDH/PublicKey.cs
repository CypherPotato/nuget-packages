using Cryptonite.ECDH.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptonite.ECDH
{
    public unsafe readonly ref struct ECDHPublicKey
    {
        internal readonly Span<byte> _keyBytes;

        public ECDHPublicKey(byte* pPublicKeyBytes)
        {
            _keyBytes = new Span<byte>(pPublicKeyBytes, 32);
        }

        public ECDHPublicKey(byte[] publicKeyBytes)
        {
            if (publicKeyBytes.Length != 32) throw new ArgumentException("Public key byte length should be exact 32 bytes-long.");
            _keyBytes = new Span<byte>(publicKeyBytes);
        }

        public ECDHPublicKey(Span<byte> publicKeyBytes)
        {
            if (publicKeyBytes.Length != 32) throw new ArgumentException("Public key byte length should be exact 32 bytes-long.");
            _keyBytes = publicKeyBytes;
        }

        public ECDHPublicKey(ECDHPrivateKey derivePrivateKey)
        {
            _keyBytes = ECDHAlgorithmService.GetPublicKey(derivePrivateKey._keyBytes);
        }

        public override string ToString() => string.Join("", GetBytes().Select(b => b.ToString("x2")));

        public byte[] GetBytes() => _keyBytes.ToArray();
    }
}
