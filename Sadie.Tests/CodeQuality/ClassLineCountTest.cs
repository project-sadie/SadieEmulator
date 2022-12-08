namespace Sadie.Tests.Other;

public class ClassLineCountTest
{
    private const int MaxLinesForClass = 400;
 
    private readonly List<string> _excludedFiles = new();
    
    /// <summary>
    /// Ensures all classes don't exceed X lines of code
    /// </summary>
    [Test]
    public Task Check_All_Classes_Line_Count()
    {
        var path = Directory.GetParent(Directory.GetCurrentDirectory())?.Parent?.Parent.Parent.FullName;
        
        foreach (var file in Directory.GetFiles(path, "*.cs", SearchOption.AllDirectories))
        {
            if (_excludedFiles.Contains(Path.GetFileName(file)))
            {
                continue;
            }
            
            Assert.That(File.ReadAllLines(file), Has.Length.LessThanOrEqualTo(MaxLinesForClass));
        }
        
        return Task.CompletedTask;
    }
}