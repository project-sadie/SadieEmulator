using Sadie.Shared.Extensions;

namespace Sadie.Tests.Shared.Extensions;

public class StringExtensionsTests
{
    [Test]
    public void Truncate_TooLong_Truncated()
    {
        Assert.That("testing".Truncate(4), Is.EqualTo("test"));
    }
    
    [Test]
    public void Truncate_AllowedLength_Untouched()
    {
        Assert.That("testing".Truncate(8), Is.EqualTo("testing"));
    }
}