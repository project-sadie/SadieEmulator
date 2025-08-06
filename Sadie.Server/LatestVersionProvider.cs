using System.Xml.Linq;

namespace SadieEmulator;

public static class LatestVersionProvider
{
    public static async Task<Version?> GetLatestVersionAsync()
    {
        try
        {
            using var client = new HttpClient();
            var xmlContent = await client.GetStringAsync(
                "https://raw.githubusercontent.com/project-sadie/SadieEmulator/refs/heads/main/Sadie.Server/Sadie.Server.csproj");

            var doc = XDocument.Parse(xmlContent);
            var versionElement = doc.Descendants("AssemblyVersion").FirstOrDefault();

            return versionElement == null ? null : Version.Parse(versionElement!.Value);
        }
        catch (Exception)
        {
            return null;
        }
    }
}