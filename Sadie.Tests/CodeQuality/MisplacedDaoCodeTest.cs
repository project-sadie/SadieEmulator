namespace Sadie.Tests.Other;

public class MisplacedDaoCodeTest
{
    private readonly List<string> ignoredFiles = new()
    {
        "IDatabaseProvider.cs",
        "DatabaseProvider.cs",
        "IDatabaseConnection.cs",
        "DatabaseConnection.cs",
        "DatabaseServiceCollection.cs",
        "Server.cs",
        "MisplacedDaoCodeTest.cs",
    };
    
    /// <summary>
    /// Ensures no database queries are ran outside of dao classes
    /// </summary>
    [Test]
    public Task No_Non_Dao_Class_Should_Have_Database_Access_Code()
    {
        var path = Directory.GetParent(Directory.GetCurrentDirectory())?.Parent?.Parent.Parent.FullName;
        
        foreach (var file in Directory.GetFiles(path, "*.cs", SearchOption.AllDirectories).Where(x => !x.EndsWith("Dao.cs") && !ignoredFiles.Contains(Path.GetFileName(x))))
        {
            foreach (var line in File.ReadAllLines(file))
            {
                Assert.That(line, Does.Not.Contain("IDatabaseProvider"));
                Assert.That(line, Does.Not.Contain("ExecuteReader"));
                Assert.That(line, Does.Not.Contain("\"SELECT"));
            }
        }
        
        return Task.CompletedTask;
    }
}