namespace Sadie.Networking.Encryption;

public class ARC4
{
    private int i;
    private int j;
    private readonly byte[] bytes = new byte[POOLSIZE];

    public const int POOLSIZE = byte.MaxValue + 1;

    public ARC4(byte[] key)
    {
        bytes = new byte[POOLSIZE];
        Initialize(key);
    }

    public void Initialize(byte[] key)
    {
        for (i = 0; i < POOLSIZE; ++i)
        {
            bytes[i] = (byte)i;
        }

        i = 0;
        j = 0;

        for (i = 0; i < POOLSIZE; ++i)
        {
            j = (j + bytes[i] + key[i % key.Length]) & (POOLSIZE - 1);
            (bytes[j], bytes[i]) = (bytes[i], bytes[j]);
        }

        i = 0;
        j = 0;
    }

    public void Parse(byte[] src)
    {
        for (int k = 0; k < src.Length; k++)
        {
            src[k] ^= Next();
        }
    }

    private byte Next()
    {
        i = ++i & (POOLSIZE - 1);
        j = (j + bytes[i]) & (POOLSIZE - 1);
        (bytes[j], bytes[i]) = (bytes[i], bytes[j]);
        return bytes[(bytes[i] + bytes[j]) & 255];
    }
}
