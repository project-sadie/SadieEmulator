using Sadie.Networking.Encryption.Extensions;
using System.Numerics;

namespace Sadie.Networking.Encryption;

public class DiffieHellman
{
    private static readonly int DH_PRIMES_BIT_SIZE = 128;
    private static readonly int DH_KEY_BIT_SIZE = 128;

    public BigInteger Prime { get; private set; }
    public BigInteger Generator { get; private set; }

    private BigInteger PrivateKey;
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
        Prime = BigIntegerExtensions.GeneratePseudoPrime(DH_PRIMES_BIT_SIZE, 10);
        Generator = BigIntegerExtensions.GeneratePseudoPrime(DH_PRIMES_BIT_SIZE, 10);

        if (Generator > Prime) (Generator, Prime) = (Prime, Generator);
    }

    public void GenerateKeys()
    {
        PrivateKey = BigIntegerExtensions.GeneratePseudoPrime(DH_KEY_BIT_SIZE, 10);
        PublicKey = BigInteger.ModPow(Generator, PrivateKey, Prime);
    }

    public BigInteger CalculateSharedKey(BigInteger publicKey)
    {
        return BigInteger.ModPow(publicKey, PrivateKey, Prime);
    }
}
