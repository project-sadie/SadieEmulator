using System.Numerics;

namespace Sadie.Networking.Encryption.Extensions;

public static class BigIntegerExtensions
{
    private static readonly Random Random = new();

    public static byte[] PerformCalculation(this byte[] src, RSACalculateDelegate method)
    {
        Array.Reverse(src);
        BigInteger data = new(src);

        data = method(data);

        var result = data.ToByteArray();
        Array.Reverse(result);

        return result;
    }

    public static BigInteger GeneratePseudoPrime(int bitLength, int certainty)
    {
        var bytes = new byte[(bitLength + 7) / 8];

        BigInteger result;
        do
        {
            Random.NextBytes(bytes);
            bytes[^1] &= 127;
            result = new BigInteger(bytes);
        }
        while (result.IsProbablePrime(certainty));

        return result;
    }

    private static bool IsProbablePrime(this BigInteger source, int certainty)
    {
        if (source == 2 || source == 3)
        {
            return true;
        }

        if (source < 2 || source % 2 == 0)
        {
            return false;
        }

        var d = source - 1;
        var s = 0;

        while (d % 2 == 0)
        {
            d /= 2;
            s += 1;
        }

        for (var i = 0; i < certainty; i++)
        {
            var a = RandomBigInteger(2, source - 2);
            var x = BigInteger.ModPow(a, d, source);

            if (x == 1 || x == source - 1)
            {
                continue;
            }

            for (var r = 1; r < s; r++)
            {
                x = BigInteger.ModPow(x, 2, source);

                if (x == 1)
                {
                    return false;
                }

                if (x == source - 1)
                {
                    break;
                }
            }

            if (x != source - 1)
            {
                return false;
            }
        }

        return true;
    }

    private static BigInteger RandomBigInteger(BigInteger minValue, BigInteger maxValue)
    {
        if (minValue > maxValue)
        {
            throw new ArgumentException("minValue must be less than or equal to maxValue");
        }

        var range = maxValue - minValue + 1;
        var length = range.GetByteCount();
        var buffer = new byte[length];

        BigInteger result;
        do
        {
            Random.NextBytes(buffer);
            buffer[^1] &= 127;
            result = new BigInteger(buffer);
        } while (result < minValue || result >= maxValue);

        return result;
    }
}
