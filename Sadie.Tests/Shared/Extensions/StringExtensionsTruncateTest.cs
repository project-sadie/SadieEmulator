using Sadie.Shared.Extensions;

namespace Sadie.Tests.Shared.Extensions;

public class StringExtensionsTruncateTest
{
    [Test]
    public Task StringTooLongShouldTruncate()
    {
        Assert.AreEqual("Hello World!".Truncate(10), "Hello Worl");
        return Task.CompletedTask;
    }
    
    [Test]
    public Task StringWithOkLengthShouldBeUnchanged()
    {
        Assert.AreEqual("Sadie!".Truncate(6), "Sadie!");
        Assert.AreEqual("Habbo".Truncate(8), "Habbo");
        
        return Task.CompletedTask;
    }
}