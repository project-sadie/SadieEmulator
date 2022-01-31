namespace Sadie.Database
{
    public interface IDatabaseProvider
    {
        IDatabaseConnection GetConnection();
        bool TestConnection();
    }
}