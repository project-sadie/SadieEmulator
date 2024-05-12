using System.Numerics;

namespace Sadie.Networking.Encryption.Extensions;

public static class BigIntegerExtensions
{
    private static readonly Random random = new();

    public static byte[] PerformCalculation(this byte[] src, RSACalculateDelegate method)
    {
        // Big integer requires little endian order!
        Array.Reverse(src);
        BigInteger data = new(src);

        data = method(data);

        byte[] result = data.ToByteArray();
        Array.Reverse(result);

        return result;
    }

    public static BigInteger GeneratePseudoPrime(int bitLength, int certainty)
    {
        byte[] bytes = new byte[(bitLength + 7) / 8];

        BigInteger result;
        do
        {
            random.NextBytes(bytes);
            bytes[^1] &= 0x7F; // Ensure non-negative
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

        BigInteger d = source - 1;
        int s = 0;

        while (d % 2 == 0)
        {
            d /= 2;
            s += 1;
        }

        for (int i = 0; i < certainty; i++)
        {
            BigInteger a = RandomBigInteger(2, source - 2);
            BigInteger x = BigInteger.ModPow(a, d, source);

            if (x == 1 || x == source - 1)
            {
                continue;
            }

            for (int r = 1; r < s; r++)
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

        BigInteger range = maxValue - minValue + 1;
        int length = range.GetByteCount();
        byte[] buffer = new byte[length];

        BigInteger result;
        do
        {
            random.NextBytes(buffer);
            buffer[^1] &= 0x7F; // Ensure non-negative
            result = new BigInteger(buffer);
        } while (result < minValue || result >= maxValue);

        return result;
    }
}
