using Sadie.Core.Enums.Game.Rooms.Users;
using Sadie.Game.Rooms.Services;

namespace Sadie.Tests.Game.Rooms.Services;

[TestFixture]
public class RoomHelperServiceTests
{
    private RoomHelperService _service;

    [SetUp]
    public void SetUp()
    {
        _service = new RoomHelperService();
    }

    [TestCase(":)", RoomUserEmotion.Smile)]
    [TestCase(":-)", RoomUserEmotion.Smile)]
    [TestCase(":]", RoomUserEmotion.Smile)]
    [TestCase(":d", RoomUserEmotion.Smile)]
    public void GetEmotionFromMessage_HappyEmojis_ReturnsSmile(string message, RoomUserEmotion expected)
    {
        Assert.That(_service.GetEmotionFromMessage(message), Is.EqualTo(expected));
    }

    [TestCase(":@", RoomUserEmotion.Angry)]
    [TestCase(">:(", RoomUserEmotion.Angry)]
    public void GetEmotionFromMessage_AngryEmojis_ReturnsAngry(string message, RoomUserEmotion expected)
    {
        Assert.That(_service.GetEmotionFromMessage(message), Is.EqualTo(expected));
    }

    [TestCase(":o", RoomUserEmotion.Shocked)]
    [TestCase(":0", RoomUserEmotion.Shocked)]
    [TestCase("o.o", RoomUserEmotion.Shocked)]
    [TestCase("0.0", RoomUserEmotion.Shocked)]
    public void GetEmotionFromMessage_ShockedEmojis_ReturnsShocked(string message, RoomUserEmotion expected)
    {
        Assert.That(_service.GetEmotionFromMessage(message), Is.EqualTo(expected));
    }

    [TestCase(":(", RoomUserEmotion.Sad)]
    [TestCase(":-(", RoomUserEmotion.Sad)]
    [TestCase(":[", RoomUserEmotion.Sad)]
    public void GetEmotionFromMessage_SadEmojis_ReturnsSad(string message, RoomUserEmotion expected)
    {
        Assert.That(_service.GetEmotionFromMessage(message), Is.EqualTo(expected));
    }

    [Test]
    public void GetEmotionFromMessage_NoEmoji_ReturnsNone()
    {
        Assert.That(_service.GetEmotionFromMessage("hello world"), Is.EqualTo(RoomUserEmotion.None));
    }

    [Test]
    public void GetEmotionFromMessage_CaseInsensitive_ReturnsSmile()
    {
        Assert.That(_service.GetEmotionFromMessage(":D"), Is.EqualTo(RoomUserEmotion.Smile));
    }

    [Test]
    public void GetEmotionFromMessage_EmojiInLongerMessage_ReturnsCorrect()
    {
        Assert.That(_service.GetEmotionFromMessage("hey there :) how are you"), Is.EqualTo(RoomUserEmotion.Smile));
    }
}
