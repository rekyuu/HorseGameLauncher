using System.Text.Json.Serialization;

namespace HorseGameLauncher.Models;

public class Components
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("uri")]
    public string? Uri { get; set; }
}

public class ComponentsDxvk : Components
{
    [JsonPropertyName("version")]
    public string? Version { get; set; }
}

public class ComponentsWine : Components
{
    [JsonPropertyName("files")]
    public ComponentsWineFiles? Files { get; set; }
}

public class ComponentsWineFiles
{
    [JsonPropertyName("wine")]
    public string? Wine { get; set; }

    [JsonPropertyName("wine64")]
    public string? Wine64 { get; set; }

    [JsonPropertyName("wineserver")]
    public string? WineServer { get; set; }

    [JsonPropertyName("wineboot")]
    public string? WineBoot { get; set; }
}