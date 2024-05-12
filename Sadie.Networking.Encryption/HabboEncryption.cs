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
        byte[] bytes = Encoding.Default.GetBytes(message);
        byte[] encryptedBytes = _crypto.Encrypt(bytes, true);
        return encryptedBytes.ToHexString();
    }

    private string GetRSADecryptedString(string data)
    {
        byte[] bytes = data.ToBytes();
        byte[] decryptedBytes = _crypto.Decrypt(bytes, true);

        return Encoding.Default.GetString(decryptedBytes);
    }

    public string GetRSADiffieHellmanPrimeKey()
    {
        string key = _diffieHellman.Prime.ToString();
        return GetRSAEncryptedString(key);
    }

    public string GetRSADiffieHellmanGeneratorKey()
    {
        string key = _diffieHellman.Generator.ToString();
        return GetRSAEncryptedString(key);
    }

    public string GetRSADiffieHellmanPublicKey()
    {
        string key = _diffieHellman.PublicKey.ToString();
        return GetRSAEncryptedString(key);
    }

    public byte[] CalculateDiffieHellmanSharedKey(string publicKey)
    {
        publicKey = GetRSADecryptedString(publicKey);
        var sharedKey = _diffieHellman.CalculateSharedKey(BigInteger.Parse(publicKey));
        byte[] result = sharedKey.ToByteArray();
        Array.Reverse(result);
        return result;
    }
}
