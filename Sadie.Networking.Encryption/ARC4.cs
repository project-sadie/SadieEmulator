namespace Sadie.Networking.Encryption;

public class Arc4
{
    private int _i;
    private int _j;
    
    private readonly byte[] _bytes = new byte[PoolSize];

    public const int PoolSize = byte.MaxValue + 1;

    public Arc4(byte[] key)
    {
        _bytes = new byte[PoolSize];
        Initialize(key);
    }

    public void Initialize(byte[] key)
    {
        for (_i = 0; _i < PoolSize; ++_i)
        {
            _bytes[_i] = (byte)_i;
        }

        _i = 0;
        _j = 0;

        for (_i = 0; _i < PoolSize; ++_i)
        {
            _j = (_j + _bytes[_i] + key[_i % key.Length]) & (PoolSize - 1);
            (_bytes[_j], _bytes[_i]) = (_bytes[_i], _bytes[_j]);
        }

        _i = 0;
        _j = 0;
    }

    public void Parse(byte[] src)
    {
        for (var k = 0; k < src.Length; k++)
        {
            src[k] ^= Next();
        }
    }

    private byte Next()
    {
        _i = ++_i & (PoolSize - 1);
        _j = (_j + _bytes[_i]) & (PoolSize - 1);
        (_bytes[_j], _bytes[_i]) = (_bytes[_i], _bytes[_j]);

        return _bytes[(_bytes[_i] + _bytes[_j]) & 255];
    }
}
