using BaseLib.Config;

namespace TH_Youmu.Scripts.Main;

[ConfigHoverTipsByDefault]
public sealed class YoumuModConfig : SimpleModConfig
{
	[ConfigSection("Cards")]
	[ConfigHoverTip]
	public static bool SfwCardArt { get; set; } = false;
}
