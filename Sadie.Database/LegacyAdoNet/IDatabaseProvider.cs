namespace Sadie.Database.LegacyAdoNet;

public interface IDatabaseProvider
{
    IDatabaseConnection GetConnection();
    bool TestConnection();
}