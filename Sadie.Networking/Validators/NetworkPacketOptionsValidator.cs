using Microsoft.Extensions.Options;
using Sadie.Networking.Options;

namespace Sadie.Networking.Validators;

public class NetworkPacketOptionsValidator : IValidateOptions<NetworkPacketOptions>
{
    public ValidateOptionsResult Validate(string? name, NetworkPacketOptions options)
    {
        return ValidateOptionsResult.Success;
    }
}
