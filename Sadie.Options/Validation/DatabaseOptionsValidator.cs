using Microsoft.Extensions.Options;
using Sadie.Options.Models;

namespace Sadie.Options.Validation;

public class DatabaseOptionsValidator : IValidateOptions<DatabaseOptions>
{
    public ValidateOptionsResult Validate(string? name, DatabaseOptions options)
    {
        if (options == null)
        {
            return ValidateOptionsResult.Fail("Configuration object is null.");
        }

        if (string.IsNullOrWhiteSpace(options.Host))
        {
            return ValidateOptionsResult.Fail("Host cannot be null or empty.");
        }

        if (string.IsNullOrWhiteSpace(options.Username))
        {
            return ValidateOptionsResult.Fail("Username cannot be null or empty.");
        }

        if (string.IsNullOrWhiteSpace(options.Password))
        {
            return ValidateOptionsResult.Fail("Password cannot be null or empty.");
        }

        if (string.IsNullOrWhiteSpace(options.Database))
        {
            return ValidateOptionsResult.Fail("Database cannot be null or empty.");
        }

        return ValidateOptionsResult.Success;
    }
}
