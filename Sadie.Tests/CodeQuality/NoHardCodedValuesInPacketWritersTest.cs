namespace Sadie.Tests.Other;

public class NoHardCodedValuesInPacketWritersTest
{
    private readonly List<string> _excludedFiles = new()
    {
        "GameAchievementsListWriter.cs",
        "HotelViewBonusRareWriter.cs",
        "ModerationToolsWriter.cs",
        "NavigatorLiftedRoomsWriter.cs",
        "NavigatorPromotedRoomsWriter.cs",
        "NavigatorSearchResultPagesWriter.cs",
        "RoomCategoriesWriter.cs",
        "PlayerAchievementsWriter.cs",
        "PlayerIgnoredUsersWriter.cs",
        "PlayerProfileWriter.cs",
        "RoomForwardDataWriter.cs",
        "RoomPromotionWriter.cs",
        "RoomSettingsWriter.cs",
        "RoomUserDataWriter.cs",
        "PlayerClothingListWriter.cs",
        "PlayerEffectListWriter.cs",
        "PlayerFriendsListWriter.cs",
        "PlayerRemoveFriendsWriter.cs",
        "PlayerSearchResultWriter.cs",
        "HabboClubGiftsWriter.cs",
        "HabboClubDataWriter.cs",
        "PlayerClubCenterDataWriter.cs",
        "PlayerClubMembershipWriter.cs",
        "PlayerDataWriter.cs",
        "PlayerPerksWriter.cs",
        "CatalogGiftConfigWriter.cs",
    };
    
    /// <summary>
    /// Ensures all classes don't exceed X lines of code
    /// </summary>
    [Test]
    public Task Check_All_Writers()
    {
        var path = Directory.GetParent(Directory.GetCurrentDirectory())?.Parent?.Parent.Parent.FullName;
        
        foreach (var file in Directory.GetFiles(path, "*Writer.cs", SearchOption.AllDirectories))
        {
            if (_excludedFiles.Contains(Path.GetFileName(file)))
            {
                continue;
            }
            
            Assert.That(File.ReadAllText(file), Does.Not.Contain("WriteBool(true);"));
            Assert.That(File.ReadAllText(file), Does.Not.Contain("WriteBool(false);"));
            Assert.That(File.ReadAllText(file), Does.Not.Contain("WriteString(\\"));
            Assert.That(File.ReadAllText(file), Does.Not.Contain("WriteInteger(0"));
            Assert.That(File.ReadAllText(file), Does.Not.Contain("WriteInteger(1"));
            Assert.That(File.ReadAllText(file), Does.Not.Contain("WriteInteger(2"));
            Assert.That(File.ReadAllText(file), Does.Not.Contain("WriteInteger(3"));
            Assert.That(File.ReadAllText(file), Does.Not.Contain("WriteInteger(4"));
            Assert.That(File.ReadAllText(file), Does.Not.Contain("WriteInteger(5"));
            Assert.That(File.ReadAllText(file), Does.Not.Contain("WriteInteger(6"));
            Assert.That(File.ReadAllText(file), Does.Not.Contain("WriteInteger(7"));
            Assert.That(File.ReadAllText(file), Does.Not.Contain("WriteInteger(8"));
            Assert.That(File.ReadAllText(file), Does.Not.Contain("WriteInteger(9"));
            Assert.That(File.ReadAllText(file), Does.Not.Contain("WriteShort(0"));
            Assert.That(File.ReadAllText(file), Does.Not.Contain("WriteShort(1"));
            Assert.That(File.ReadAllText(file), Does.Not.Contain("WriteShort(2"));
            Assert.That(File.ReadAllText(file), Does.Not.Contain("WriteShort(3"));
            Assert.That(File.ReadAllText(file), Does.Not.Contain("WriteShort(4"));
            Assert.That(File.ReadAllText(file), Does.Not.Contain("WriteShort(5"));
            Assert.That(File.ReadAllText(file), Does.Not.Contain("WriteShort(6"));
            Assert.That(File.ReadAllText(file), Does.Not.Contain("WriteShort(7"));
            Assert.That(File.ReadAllText(file), Does.Not.Contain("WriteShort(8"));
            Assert.That(File.ReadAllText(file), Does.Not.Contain("WriteShort(9"));
        }
        
        return Task.CompletedTask;
    }
}