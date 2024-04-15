using Microsoft.Extensions.Options;
using Sadie.Options.Models;

namespace Sadie.Options.Validation;

public class NetworkPacketOptionsValidator : IValidateOptions<NetworkPacketOptions>
{
    public ValidateOptionsResult Validate(string? name, NetworkPacketOptions options)
    {
        if (options == null)
        {
            return ValidateOptionsResult.Fail("Configuration object is null.");
        }

        return ValidateOptionsResult.Success;
    }
}
