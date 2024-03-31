namespace Sadie.Tests.CodeQuality;

public class ClassLineLengthTest
{
    [Test]
    public Task Check_All_Classes_Line_Length()
    {
        var path = Directory.GetParent(Directory.GetCurrentDirectory())?.Parent?.Parent.Parent.FullName;
        
        foreach (var file in Directory.GetFiles(path, "*.cs", SearchOption.AllDirectories))
        {
            var lines = File.ReadAllLines(file);

            foreach (var line in lines)
            {
                if (line.TrimStart(' ').StartsWith("//"))
                {
                    continue;
                }
                
                if (line.Length >= TestSettings.MaxCharactersForLine)
                {
                    Console.WriteLine(file);
                }
                
                Assert.That(line.Length, Is.LessThan(TestSettings.MaxCharactersForLine));
            }
        }
        
        return Task.CompletedTask;
    }
}