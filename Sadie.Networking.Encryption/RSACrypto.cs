using Sadie.Networking.Encryption.Extensions;
using System.Globalization;
using System.Numerics;
using System.Security.Cryptography;

namespace Sadie.Networking.Encryption;

public class RsaCrypto(string exponent, string modules, string privateExponent)
{
    private readonly BigInteger _exponent = BigInteger.Parse(exponent, NumberStyles.HexNumber);
    private readonly BigInteger _modules = BigInteger.Parse(modules, NumberStyles.HexNumber);
    private readonly BigInteger _privateExponent = BigInteger.Parse(privateExponent, NumberStyles.HexNumber);

    private int GetBlockSize()
    {
        return ((int)_modules.GetBitLength() + 7) / 8;
    }

    public byte[] Encrypt(byte[] src, bool isPrivate = false)
    {
        return DoEncrypt(src, isPrivate ? DoPrivate : DoPublic);
    }

    public byte[] Decrypt(byte[] src, bool isPrivate = false)
    {
        return DoDecrypt(src, isPrivate ? DoPrivate : DoPublic);
    }

    protected BigInteger DoPublic(BigInteger src)
    {
        return BigInteger.ModPow(src, _exponent, _modules);
    }

    protected BigInteger DoPrivate(BigInteger src)
    {
        return BigInteger.ModPow(src, _privateExponent, _modules);
    }

    private byte[] DoEncrypt(byte[] src, RsaCalculateDelegate method)
    {
        return Pkcs1Pad(src).PerformCalculation(method);
    }

    private byte[] DoDecrypt(byte[] src, RsaCalculateDelegate method)
    {
        if (src.Length > GetBlockSize())
        {
            throw new ArgumentException("Src is to long to encrypt.");
        }

        return Pkcs1Unpad(src.PerformCalculation(method));
    }

    private byte[] Pkcs1Pad(IReadOnlyList<byte> src)
    {
        var n = GetBlockSize();
        var bytes = new byte[n];

        var i = src.Count - 1;

        while (i >= 0 && n > 11)
        {
            bytes[--n] = src[i--];
        }

        bytes[--n] = 0;

        while (n > 2)
        {
            bytes[--n] = 255;
        }

        bytes[--n] = 1;
        bytes[--n] = 0;

        return bytes;
    }

    private byte[] Pkcs1Unpad(byte[] src)
    {
        if (src[0] == 2)
        {
            var temp = new byte[src.Length + 1];
            Array.Copy(src, 0, temp, 1, src.Length);
            src = temp;
        }

        if (src[0] != 0 || src[1] != 2)
        {
            throw new CryptographicException("PKCS v1.5 Decode Error");
        }

        var startIndex = 2;
        do
        {
            if (src.Length < startIndex)
            {
                throw new CryptographicException("PKCS v1.5 Decode Error");
            }
        }
        while (src[startIndex++] != 0);

        var bytes = new byte[src.Length - startIndex];
        Array.Copy(src, startIndex, bytes, 0, bytes.Length);

        return bytes;
    }
}
