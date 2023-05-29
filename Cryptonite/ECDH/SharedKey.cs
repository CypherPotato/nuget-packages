using Cryptonite.ECDH.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptonite.ECDH
{
    public ref struct ECDHSharedKey
    {
        private Span<byte> _keyBytes;

        internal ECDHSharedKey(ECDHPrivateKey secretKey, ECDHPublicKey peerPublicKey)
        {
            _keyBytes = ECDHAlgorithmService.GetSharedSecretKey(peerPublicKey, secretKey);
        }

        public byte[] GetBytes() => _keyBytes.ToArray();
        public override string ToString() => string.Join("", GetBytes().Select(b => b.ToString("x2")));
    }
}
