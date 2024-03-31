namespace Sadie.Tests.CodeQuality;

public class ClassLineCountTest
{
    [Test]
    public Task Check_All_Classes_Line_Count()
    {
        var path = Directory.GetParent(Directory.GetCurrentDirectory())?.Parent?.Parent.Parent.FullName;
        
        foreach (var file in Directory.GetFiles(path, "*.cs", SearchOption.AllDirectories))
        {
            var lines = File.ReadAllLines(file);

            if (lines.Length > TestSettings.MaxLinesForClass)
            {
                Console.WriteLine(file);
            }
            
            Assert.That(lines, Has.Length.LessThanOrEqualTo(TestSettings.MaxLinesForClass));
        }
        
        return Task.CompletedTask;
    }
}