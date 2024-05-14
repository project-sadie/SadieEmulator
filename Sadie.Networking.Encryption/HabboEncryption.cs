using Microsoft.Extensions.Options;
using Sadie.Networking.Encryption.Extensions;
using Sadie.Options.Options;
using System.Numerics;
using System.Text;

namespace Sadie.Networking.Encryption;

public class HabboEncryption(IOptions<EncryptionOptions> options)
{
    private readonly RSACrypto _crypto = new(options.Value.E, options.Value.N, options.Value.D);
    private readonly DiffieHellman _diffieHellman = new();

    private string GetRSAEncryptedString(string message)
    {
        var bytes = Encoding.Default.GetBytes(message);
        var encryptedBytes = _crypto.Encrypt(bytes, true);

        return encryptedBytes.ToHexString();
    }

    private string GetRSADecryptedString(string data)
    {
        var bytes = data.ToBytes();
        var decryptedBytes = _crypto.Decrypt(bytes, true);

        return Encoding.Default.GetString(decryptedBytes);
    }

    public string GetRSADiffieHellmanPrimeKey()
    {
        var key = _diffieHellman.Prime.ToString();
        return GetRSAEncryptedString(key);
    }

    public string GetRSADiffieHellmanGeneratorKey()
    {
        var key = _diffieHellman.Generator.ToString();
        return GetRSAEncryptedString(key);
    }

    public string GetRSADiffieHellmanPublicKey()
    {
        var key = _diffieHellman.PublicKey.ToString();
        return GetRSAEncryptedString(key);
    }

    public byte[] CalculateDiffieHellmanSharedKey(string publicKey)
    {
        publicKey = GetRSADecryptedString(publicKey);
        var sharedKey = _diffieHellman.CalculateSharedKey(BigInteger.Parse(publicKey));
        var result = sharedKey.ToByteArray();
        Array.Reverse(result);

        return result;
    }
}
