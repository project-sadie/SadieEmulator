namespace Sadie.Tests.CodeQuality;

public class MisplacedDaoCodeTest
{
    private readonly List<string> _ignoredFiles = new()
    {
        "IDatabaseProvider.cs",
        "DatabaseProvider.cs",
        "IDatabaseConnection.cs",
        "DatabaseConnection.cs",
        "DatabaseServiceCollection.cs",
        "MisplacedDaoCodeTest.cs"
    };
    
    /// <summary>
    /// Ensures no database queries are ran outside of dao classes
    /// </summary>
    [Test]
    public Task No_Non_Dao_Class_Should_Have_Database_Access_Code()
    {
        var path = Directory.GetParent(Directory.GetCurrentDirectory())?.Parent?.Parent.Parent.FullName;
        
        var files = Directory.GetFiles(path, "*.cs", SearchOption.AllDirectories)
            .Where(x => !x.EndsWith("Dao.cs") && !_ignoredFiles.Contains(Path.GetFileName(x)));
        
        foreach (var file in files)
        {
            foreach (var line in File.ReadAllLines(file))
            {
                Assert.That(line, Does.Not.Contain("IDatabaseProvider "));
                Assert.That(line, Does.Not.Contain("ExecuteReader("));
                Assert.That(line, Does.Not.Contain("\"SELECT"));
                Assert.That(line, Does.Not.Contain("\"INSERT INTO"));
            }
        }
        
        return Task.CompletedTask;
    }
}