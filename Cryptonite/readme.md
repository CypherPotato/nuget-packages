# Cryptonite

Written by CypherPotato

Cryptonite is an .NET library which provides cryptographic utilities. It
is cross-platform, functional and simple to use, without requiring much
technical knowledge of cryptography and easy to use.

# Functions

- `Cryptonite.StaticOperations.TimingSafeEqual`: evaluates an timing-attack-resistant comparison between spans A and B.
- `Cryptonite.StaticOperations.Range`: gets an span range from another span.
- `Cryptonite.StaticOperations.PadZeros`: pads leading or trailing bytes from another span.
- `Cryptonite.StaticOperations.Copy`: copies contents from A to B at specified offset.
- `Cryptonite.StaticOperations.RefXorGate`: performs an xor operation in A using B.
- `Cryptonite.StaticOperations.RefRandomSecureBytes`: gets an random byte array in an existing span.
- `Cryptonite.StaticOperations.XorGate`: returns an span resulted from a xor operation between the two arrays A^B.
- `Cryptonite.StaticOperations.RandomSecureBytes`: gets an new array with secure random bytes at specified length.
- `Cryptonite.StaticOperations.ScryptDerive`: creates an SCRYPT key derive from an key, at an specified length.
- `Cryptonite.StaticOperations.Pbkdf2Derive`: creates an hashing key derive from an key, at an specified length.

# Credits

Some old code has been updated to be compatible with new features of the latest framework by me,
however all the heavy lifting is dedicated to the original creators of the implementations.

- The ECDH implementation is based in the work of [frankigamez/ECDH-Curve25519](https://github.com/frankigamez/ECDH-Curve25519) (MIT) and Daniel J. Bernstein.
- CryptSharp, by James F. Bellinger, available on https://www.zer7.com/software/cryptsharp.