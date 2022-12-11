namespace Sadie.Tests.Other;

public class ClassLineLengthTest
{
    private const int MaxLengthForLine = 155;
 
    private readonly List<string> _excludedFiles = new();
    
    /// <summary>
    /// Ensures no line of code exceeds MaxLengthForLine characters
    /// </summary>
    /// <returns></returns>
    [Test]
    public Task Check_All_Classes_Line_Length()
    {
        var path = Directory.GetParent(Directory.GetCurrentDirectory())?.Parent?.Parent.Parent.FullName;
        
        foreach (var file in Directory.GetFiles(path, "*.cs", SearchOption.AllDirectories))
        {
            if (_excludedFiles.Contains(Path.GetFileName(file)))
            {
                continue;
            }

            var lines = File.ReadAllLines(file);

            foreach (var line in lines)
            {
                if (line.TrimStart(' ').StartsWith("//"))
                {
                    continue;
                }
                
                if (line.Length >= MaxLengthForLine)
                {
                    Console.WriteLine(file);
                }
                
                Assert.That(line.Length, Is.LessThan(MaxLengthForLine));
            }
        }
        
        return Task.CompletedTask;
    }
}