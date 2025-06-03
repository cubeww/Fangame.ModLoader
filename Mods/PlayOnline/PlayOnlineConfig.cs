using System.ComponentModel;
using System.Text.Json.Serialization;
using Fangame.ModLoader;

namespace PlayOnline;

public class PlayOnlineConfig : ModConfig
{
    [Category("Mod")]
    [ReadOnly(true)]
    [Description("Convert fangame to online mode.")]
    [JsonIgnore]
    public string Version { get; set; } = "Beta";

    [Category("Server")]
    public string ServerIp { get; set; } = "iwannastartaparty.com";

    [Category("Server")]
    public int TcpPort { get; set; } = 8002;

    [Category("Server")]
    public int UdpPort { get; set; } = 8003;

    [Category("Game")]
    public string PlayerName { get; set; } = "Player";

    [Category("Game")]
    public string Password { get; set; } = "";

    [Category("Game")]
    public bool RaceMode { get; set; }
}
