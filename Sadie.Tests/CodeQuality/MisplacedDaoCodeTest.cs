namespace Sadie.Tests.Other;

public class MisplacedDaoCodeTest
{
    /// <summary>
    /// Ensures no database queries are ran outside of dao classes
    /// </summary>
    [Test]
    public Task No_Non_Dao_Class_Should_Have_Database_Access_Code()
    {
        var ignoredFiles = new List<string>
        {
            "IDatabaseProvider.cs",
            "DatabaseProvider.cs",
            "IDatabaseConnection.cs",
            "DatabaseConnection.cs",
            "DatabaseServiceCollection.cs",
            "Server.cs",
        };
        
        var path = Directory.GetParent(Directory.GetCurrentDirectory())?.Parent?.Parent.Parent.FullName;
        
        foreach (var file in Directory.GetFiles(path, "*.cs", SearchOption.AllDirectories))
        {
            var fileName = Path.GetFileName(file);
            
            if (file.Contains("Sadie.Tests") || file.EndsWith("Dao.cs") || ignoredFiles.Contains(fileName))
            {
                continue;
            }
            
            Console.WriteLine(file);
            
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