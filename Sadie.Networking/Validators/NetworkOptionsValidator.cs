using Microsoft.Extensions.Options;
using Sadie.Networking.Options;

namespace Sadie.Networking.Validators;

public class NetworkOptionsValidator : IValidateOptions<NetworkOptions>
{
    public ValidateOptionsResult Validate(string? name, NetworkOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.Host))
        {
            return ValidateOptionsResult.Fail($"{nameof(NetworkOptions)} 'Host' cannot be null or empty.");
        }

        if (!string.IsNullOrEmpty(options.CertificateFile) && !File.Exists(options.CertificateFile))
        {
            return ValidateOptionsResult.Fail($"{nameof(NetworkOptions)} 'CertificateFile' is set but doesn't exist.");
        }

        return ValidateOptionsResult.Success;
    }
}
