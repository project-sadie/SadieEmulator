namespace Sadie.Shared.Dtos;

public class HallOfFameEntryData
{
    public int UserId { get; set; }
    public required string Username { get; set; }
    public required string FigureCode { get; set; }
    public int Rank { get; set; }
    public int CurrentScore { get; set; }
}