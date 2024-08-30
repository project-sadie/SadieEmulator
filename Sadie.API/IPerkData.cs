namespace Sadie.API;

public interface IPerkData
{
    string? Code { get; set; }
    string? ErrorMessage { get; set; }
    bool Allowed { get; set; }
}