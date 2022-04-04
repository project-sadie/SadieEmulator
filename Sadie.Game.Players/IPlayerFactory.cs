using Sadie.Database;

namespace Sadie.Game.Players;

public interface IPlayerFactory
{
    IPlayer CreateFromRecord(DatabaseRecord record, DatabaseReader savedSearchesReader, DatabaseReader permissionsReader);
    IPlayer CreateFromBasicRecord(DatabaseRecord record);
}