using Sadie.Core.Shared.Extensions;

namespace Sadie.Tests.Shared.Extensions;

[TestFixture]
public class DateTimeExtensionsTests
{
    [Test]
    public void ToUnix_UnixEpoch_ReturnsZero()
    {
        var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        Assert.That(epoch.ToUnix(), Is.EqualTo(0));
    }

    [Test]
    public void ToUnix_KnownDate_ReturnsExpectedTimestamp()
    {
        var date = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        Assert.That(date.ToUnix(), Is.EqualTo(946684800));
    }

    [Test]
    public void ToUnix_MinValue_ReturnsNegative()
    {
        var min = DateTime.SpecifyKind(DateTime.MinValue, DateTimeKind.Utc);
        Assert.That(min.ToUnix(), Is.LessThan(0));
    }
}
