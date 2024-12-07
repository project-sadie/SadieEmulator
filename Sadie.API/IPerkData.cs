using Sadie.Shared.Attributes;

namespace Sadie.API;

public interface IPerkData
{
    [PacketData] string? Code { get; set; }
    [PacketData] string? ErrorMessage { get; set; }
    [PacketData] bool Allowed { get; set; }
}