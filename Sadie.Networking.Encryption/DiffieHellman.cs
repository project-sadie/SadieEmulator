using Sadie.Networking.Encryption.Extensions;
using System.Numerics;

namespace Sadie.Networking.Encryption;

public class DiffieHellman
{
    private static readonly int DhPrimesBitSize = 128;
    private static readonly int DhKeyBitSize = 128;

    public BigInteger Prime { get; private set; }
    public BigInteger Generator { get; private set; }

    private BigInteger _privateKey;
    public BigInteger PublicKey { get; private set; }

    public DiffieHellman()
    {
        GeneratePrimes();
        GenerateKeys();
    }

    public DiffieHellman(BigInteger prime, BigInteger generator)
    {
        Prime = prime;
        Generator = generator;
        GenerateKeys();
    }

    public void GeneratePrimes()
    {
        Prime = BigIntegerExtensions.GeneratePseudoPrime(DhPrimesBitSize, 10);
        Generator = BigIntegerExtensions.GeneratePseudoPrime(DhPrimesBitSize, 10);

        if (Generator > Prime) (Generator, Prime) = (Prime, Generator);
    }

    public void GenerateKeys()
    {
        _privateKey = BigIntegerExtensions.GeneratePseudoPrime(DhKeyBitSize, 10);
        PublicKey = BigInteger.ModPow(Generator, _privateKey, Prime);
    }

    public BigInteger CalculateSharedKey(BigInteger publicKey)
    {
        return BigInteger.ModPow(publicKey, _privateKey, Prime);
    }
}
