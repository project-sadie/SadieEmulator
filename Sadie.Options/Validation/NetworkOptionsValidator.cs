using Microsoft.Extensions.Options;
using Sadie.Options.Models;

namespace Sadie.Options.Validation;

public class NetworkOptionsValidator : IValidateOptions<NetworkOptions>
{
    public ValidateOptionsResult Validate(string? name, NetworkOptions options)
    {
        if (options == null)
        {
            return ValidateOptionsResult.Fail("Configuration object is null.");
        }

        if (string.IsNullOrWhiteSpace(options.Host))
        {
            return ValidateOptionsResult.Fail($"{nameof(NetworkOptions)} `Host` cannot be null or empty.");
        }

        return ValidateOptionsResult.Success;
    }
}
