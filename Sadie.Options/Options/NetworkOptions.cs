namespace Sadie.Options.Options;

public class NetworkOptions
{
    public string? Host { get; set; }
    public int Port { get; set; }
    public bool UseWss { get; set; }
    public string? CertificateFile { get; set; }
}
