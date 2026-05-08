using Sadie.Core.Shared.Extensions;

namespace Sadie.Tests.Shared.Extensions;

[TestFixture]
public class EnumerableExtensionsTests
{
    [Test]
    public void Batch_EvenSplit_ReturnsCorrectBatches()
    {
        var items = new[] { 1, 2, 3, 4, 5, 6 };
        var batches = items.Batch(3).ToList();

        Assert.That(batches, Has.Count.EqualTo(2));
        Assert.Multiple(() =>
        {
            Assert.That(batches[0]!.Count(), Is.EqualTo(3));
            Assert.That(batches[1]!.Count(), Is.EqualTo(3));
        });
    }

    [Test]
    public void Batch_UnevenSplit_LastBatchSmaller()
    {
        var items = new[] { 1, 2, 3, 4, 5, 6, 7 };
        var batches = items.Batch(3).ToList();

        Assert.That(batches, Has.Count.EqualTo(3));
        Assert.Multiple(() =>
        {
            Assert.That(batches[0]!.Count(), Is.EqualTo(3));
            Assert.That(batches[1]!.Count(), Is.EqualTo(3));
            Assert.That(batches[2]!.Count(), Is.EqualTo(1));
        });
    }

    [Test]
    public void Batch_EmptySource_ReturnsEmpty()
    {
        var items = Array.Empty<int>();
        var batches = items.Batch(3).ToList();

        Assert.That(batches, Is.Empty);
    }

    [Test]
    public void Batch_SizeLargerThanSource_ReturnsSingleBatch()
    {
        var items = new[] { 1, 2 };
        var batches = items.Batch(10).ToList();

        Assert.That(batches, Has.Count.EqualTo(1));
        Assert.That(batches[0]!.Count(), Is.EqualTo(2));
    }

    [Test]
    public void PickRandom_NonEmpty_ReturnsElement()
    {
        var items = new[] { 10, 20, 30, 40, 50 };
        var picked = items.PickRandom();

        Assert.That(items, Does.Contain(picked));
    }
}
