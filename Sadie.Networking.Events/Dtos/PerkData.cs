using Sadie.API;

namespace Sadie.Networking.Events.Dtos;

public class PerkData(string? code, string? errorMessage, bool allowed) : IPerkData
{
    public string? Code { get; set; } = code;
    public string? ErrorMessage { get; set; } = errorMessage;
    public bool Allowed { get; set; } = allowed;
}