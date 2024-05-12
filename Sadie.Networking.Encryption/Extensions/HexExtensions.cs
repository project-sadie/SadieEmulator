namespace Sadie.Networking.Encryption.Extensions;

public static class HexExtensions
{
    public static string ToHexString(this byte[] bytes)
    {
        return BitConverter.ToString(bytes).Replace("-", string.Empty);
    }

    public static byte[] ToBytes(this string hex)
    {
        if ((hex.Length & 1) != 0)
        {
            throw new ArgumentException("Input must have even number of characters");
        }

        byte[] bytes = new byte[hex.Length / 2];
        for (int i = 0; i < bytes.Length; i++)
        {
            int high = ParseNybble(hex[i * 2]);
            int low = ParseNybble(hex[i * 2 + 1]);
            bytes[i] = (byte)((high << 4) | low);
        }

        return bytes;
    }

    private static int ParseNybble(char c)
    {
        unchecked
        {
            uint i = (uint)(c - '0');
            if (i < 10)
                return (int)i;
            i = (c & ~0x20u) - 'A';
            if (i < 6)
                return (int)i + 10;
            throw new ArgumentException("Invalid nybble: " + c);
        }
    }
}
