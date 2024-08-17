using Microsoft.Extensions.Options;
using Sadie.Networking.Encryption.Extensions;
using Sadie.Options.Options;
using System.Numerics;
using System.Text;

namespace Sadie.Networking.Encryption;

public class HabboEncryption(IOptions<EncryptionOptions> options)
{
    private readonly RsaCrypto _crypto = new(options.Value.E, options.Value.N, options.Value.D);
    private readonly DiffieHellman _diffieHellman = new();

    private string GetRsaEncryptedString(string message)
    {
        var bytes = Encoding.Default.GetBytes(message);
        var encryptedBytes = _crypto.Encrypt(bytes, true);

        return encryptedBytes.ToHexString();
    }

    private string GetRsaDecryptedString(string data)
    {
        var bytes = data.ToBytes();
        var decryptedBytes = _crypto.Decrypt(bytes, true);

        return Encoding.Default.GetString(decryptedBytes);
    }

    public string GetRsaDiffieHellmanPrimeKey()
    {
        var key = _diffieHellman.Prime.ToString();
        return GetRsaEncryptedString(key);
    }

    public string GetRsaDiffieHellmanGeneratorKey()
    {
        var key = _diffieHellman.Generator.ToString();
        return GetRsaEncryptedString(key);
    }

    public string GetRsaDiffieHellmanPublicKey()
    {
        var key = _diffieHellman.PublicKey.ToString();
        return GetRsaEncryptedString(key);
    }

    public byte[] CalculateDiffieHellmanSharedKey(string publicKey)
    {
        publicKey = GetRsaDecryptedString(publicKey);
        var sharedKey = _diffieHellman.CalculateSharedKey(BigInteger.Parse(publicKey));
        var result = sharedKey.ToByteArray();
        Array.Reverse(result);

        return result;
    }
}
