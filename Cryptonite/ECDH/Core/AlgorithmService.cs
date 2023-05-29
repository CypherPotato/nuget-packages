using Cryptonite.ECDH.Core.Op;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Cryptonite.ECDH.Core
{
    internal static class ECDHAlgorithmService
    {
        private const int PrivateKeySizeInBytes = 32;
        private const int SharedKeySizeInBytes = 32;

        public static Span<byte> GetRandomPrivateKey()
        {
            var privateKey = new byte[PrivateKeySizeInBytes];
            RandomNumberGenerator.Create().GetBytes(privateKey);
            ClampOperation.Clamp(s: privateKey, offset: 0);
            return new Span<byte>(privateKey);
        }

        public static Span<byte> GetPublicKey(Span<byte> secretKey)
        {
            if (secretKey == null) throw new ArgumentNullException(nameof(secretKey));
            if (secretKey.Length != PrivateKeySizeInBytes) throw new ArgumentException($"{nameof(secretKey)} must be {PrivateKeySizeInBytes}");

            var publicKey = new Span<byte>(new byte[32]);
            secretKey.CopyTo(publicKey);

            ClampOperation.Clamp(publicKey);

            var a = GroupElementsOperations.ScalarMultiplicationBase(publicKey); // To MontgomeryX

            var tempX = FieldElementOperations.Add(ref a.Z, ref a.Y); //Get X
            var tempZ = FieldElementOperations.Sub(ref a.Z, ref a.Y);
            tempZ = FieldElementOperations.Invert(ref tempZ); //Get Z

            // Obtains the Public Key
            var publicKeyFieldElement = FieldElementOperations.Multiplication(ref tempX, ref tempZ); //X*Z       
            FieldElementOperations.ToBytes(publicKey, ref publicKeyFieldElement);
            return publicKey;
        }

        public static Span<byte> GetSharedSecretKey(ECDHPublicKey peerPublicKey, ECDHPrivateKey privateKey)
        {
            //Resolve SharedSecret Key using the Montgomery Elliptical Curve Operations...
            var sharedSecretKey = MontgomeryOperations.ScalarMultiplication(
                n: privateKey._keyBytes,
                p: peerPublicKey._keyBytes,
                qSize: SharedKeySizeInBytes);

            //hashes like the NaCl paper says instead i.e. HSalsa(x,0)
            sharedSecretKey = Salsa20.HSalsa20(key: sharedSecretKey);

            return sharedSecretKey;
        }
    }
}
