namespace Sadie.Options.Options;

public class NetworkOptions
{
    public string? Host { get; init; }
    public int Port { get; init; }
    public bool UseWss { get; init; }
    public string? CertificateFile { get; init; }
}
