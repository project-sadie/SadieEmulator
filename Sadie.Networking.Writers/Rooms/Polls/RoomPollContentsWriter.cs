using Sadie.API;
using Sadie.API.Networking;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers.Rooms.Polls.Dtos;

namespace Sadie.Networking.Writers.Rooms.Polls;

[PacketId(ServerPacketId.RoomPollContents)]
public class RoomPollContentsWriter : AbstractPacketWriter
{
    public required int Id { get; init; }
    public required string StartMessage { get; init; }
    public required string EndMessage { get; init; }
    public required List<RoomPollQuestion> Questions { get; init; }
    public required bool Nps { get; set; }

    public override void OnSerialize(INetworkPacketWriter writer)
    {
        writer.WriteInteger(Id);
        writer.WriteString(StartMessage);
        writer.WriteString(EndMessage);

        writer.WriteInteger(Questions.Count);
        
        foreach (var question in Questions)
        {
            WriteQuestion(question, writer);
        }
        
        writer.WriteBool(Nps);
    }

    private static void WriteQuestion(RoomPollQuestion question, INetworkPacketWriter writer)
    {
        writer.WriteInteger(question.Id);
        writer.WriteInteger(question.SortOrder);
        writer.WriteInteger(question.Type);
        writer.WriteString(question.Text);
        writer.WriteInteger(question.Category);
        writer.WriteInteger(question.AnswerType);
        writer.WriteInteger(question.AnswerCount);
        
        writer.WriteInteger(question.Choices.Count);

        foreach (var choice in question.Choices)
        {
            writer.WriteString(choice.Value);
            writer.WriteString(choice.Text);
            writer.WriteInteger(choice.Type);
        }
        
        writer.WriteInteger(question.Children.Count);

        foreach (var child in question.Children)
        {
            WriteQuestion(child, writer);
        }
    }
}
