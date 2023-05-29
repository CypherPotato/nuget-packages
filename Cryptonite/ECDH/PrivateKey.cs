using Cryptonite.ECDH.Core;
using System;
using System.Security.Cryptography.X509Certificates;

namespace Cryptonite.ECDH
{
    public unsafe ref struct ECDHPrivateKey
    {
        internal bool hasCalculatedPublicKey = false;
        internal ECDHPublicKey _publKey;
        internal Span<byte> _keyBytes;

        public ECDHPrivateKey()
        {
            _keyBytes = ECDHAlgorithmService.GetRandomPrivateKey();
        }

        public ECDHPrivateKey(byte* privateKeyBytes)
        {
            _keyBytes = new Span<byte>(privateKeyBytes, 32);
        }

        public ECDHPrivateKey(Span<byte> privateKeyBytes)
        {
            if (privateKeyBytes.Length != 32) throw new ArgumentException("Private key byte length should be exact 32 bytes-long.");
            _keyBytes = privateKeyBytes;
        }

        public ECDHPrivateKey(byte[] privateKeyBytes)
        {
            if (privateKeyBytes.Length != 32) throw new ArgumentException("Private key byte length should be exact 32 bytes-long.");
            _keyBytes = new Span<byte>(privateKeyBytes);
        }

        public byte[] GetBytes() => _keyBytes.ToArray();

        public ECDHPublicKey GetPublicKey()
        {
            if (!hasCalculatedPublicKey)
            {
                _publKey = new ECDHPublicKey(this);
                hasCalculatedPublicKey = true;
            }
            return _publKey;
        }

        public ECDHSharedKey GetSharedKey(ECDHPublicKey peerPublicKey)
        {
            return new ECDHSharedKey(this, peerPublicKey);
        }

        public override string ToString() => string.Join("", GetBytes().Select(b => b.ToString("x2")));
    }
}