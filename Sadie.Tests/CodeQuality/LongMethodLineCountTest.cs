using Microsoft.CodeAnalysis.CSharp;
#pragma warning disable CS8602 // Dereference of a possibly null reference.

namespace Sadie.Tests.CodeQuality;

public class LongMethodLineCountTest
{
    [Test]
    public Task Check_All_Methods_Line_Count()
    {
        var path = Directory.GetParent(Directory.GetCurrentDirectory())?.Parent?.Parent.Parent.FullName;
        var fails = 0;

        foreach (var file in Directory.GetFiles(path!, "*.cs", SearchOption.AllDirectories))
        {
            try
            {
                var code = File.ReadAllText(file);
                var tree = CSharpSyntaxTree.ParseText(code);
                var root = tree.GetRoot();

                var walker = new MethodWalker();
                walker.Visit(root);
            }
            catch (Exception e)
            {
                fails++;
                Console.WriteLine($"[{file.Split("/").Last().Replace(".cs", "")}] {e.Message}");
            }
        }

        Assert.That(fails, Is.EqualTo(0));
        return Task.CompletedTask;
    }
}
