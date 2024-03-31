namespace Sadie.Tests.CodeQuality;

public class LongMethodLineCountTest
{
    private const int MaxLinesForMethod = 100;

    private readonly List<string> _excluded = new()
    {
    };
    
    [Test]
    public Task Check_All_Methods_Line_Count()
    {
        var path = Directory
            .GetParent(Directory.GetCurrentDirectory())
            ?.Parent?.Parent.Parent.FullName;

        var fails = 0;

        foreach (var file in Directory.GetFiles(path, "*.cs", SearchOption.AllDirectories))
        {
            var className = file.Split("/").Last().Replace(".cs", "");
            var content = File.ReadAllText(file);

            if (!content.Contains("public class"))
            {
                continue;
            }
            
            var lines = content.Split(Environment.NewLine).Where(x => !string.IsNullOrEmpty(x));
            var lastMethodDecLineNumber = 0;
            var lastMethodDecName = string.Empty;
            var lineNumber = 0;
            var methods = new Dictionary<string, int>();

            foreach (var line in lines)
            {
                lineNumber++;

                if (line.Length < 5)
                {
                    continue;
                }

                var isMethodDecLine =
                    line.Contains("void")
                    || line.Contains("public Task")
                    || line.Contains("private Task")
                    || line.Contains("public async")
                    || line.Contains("private async");

                var isMethodEndLine = line[4..] == "}";

                if (isMethodDecLine)
                {
                    var methodName = line.Split(" ").First(x => x.Contains("(")).Split("(")[0];
                    
                    lastMethodDecLineNumber = lineNumber;
                    lastMethodDecName = $"{className}:{methodName}";
                }
                else if (isMethodEndLine && lastMethodDecLineNumber != 0)
                {
                    methods[lastMethodDecName] = lineNumber - lastMethodDecLineNumber;

                    lastMethodDecLineNumber = 0;
                }
            }

            methods = methods
                .Where(x => !_excluded.Contains(x.Key))
                .ToDictionary();

            var methodsTooBig = methods
                .Where(x => x.Value > MaxLinesForMethod)
                .ToList();

            if (methodsTooBig.Count == 0)
            {
                continue;
            }
            
            Console.WriteLine(string.Join(",", methodsTooBig.Select(x => $"{x.Key.Replace(":", " ")} | {x.Value} lines")));
                
            fails++;
        }

        Assert.That(fails, Is.EqualTo(0));
        return Task.CompletedTask;
    }
}
