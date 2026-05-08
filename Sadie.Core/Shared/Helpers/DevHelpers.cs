using System.Diagnostics;

namespace Sadie.Core.Shared.Helpers;

public class DevHelpers
{
    private static async Task MeasureAsync(Func<Task> action, int iterations = 1)
    {
        var sw = Stopwatch.StartNew();

        for (var i = 0; i < iterations; i++)
        {
            await action();
        }
        
        sw.Stop();
        Console.WriteLine($"Ran {iterations} iterations in: {sw.Elapsed}");
    }
}