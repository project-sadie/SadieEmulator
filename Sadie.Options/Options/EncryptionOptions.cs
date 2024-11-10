namespace Sadie.Options.Options;

public class EncryptionOptions
{
    public required bool Enabled { get; init; }
    public required string E { get; init; }
    public required string N { get; init; }
    public required string D { get; init; }
}
