using Microsoft.Extensions.Options;
using Sadie.Options.Options;

namespace Sadie.Options.Validation;

public class NetworkPacketOptionsValidator : IValidateOptions<NetworkPacketOptions>
{
    public ValidateOptionsResult Validate(string? name, NetworkPacketOptions options)
    {
        return ValidateOptionsResult.Success;
    }
}
