using BaseLib.Abstracts;
using Godot;

namespace TH_Youmu.Scripts.Main
{
	public class YoumuCardPool : CustomCardPoolModel
	{
		private static readonly Color DesiredShaderColor = new Color("00d3aaff");
		private static readonly Color DesiredDeckEntryCardColor = new Color("00cca9ff");

		public override string Title => "TH_Youmu";

		public override Color ShaderColor => LinearToSrgbApprox(DesiredShaderColor);
		public override Color DeckEntryCardColor => LinearToSrgbApprox(DesiredDeckEntryCardColor);
		public override string? BigEnergyIconPath => "res://TH_Youmu/ArtWorks/Character/card_orb.png";
		public override string? TextEnergyIconPath => "res://TH_Youmu/ArtWorks/Character/cost_orb.png";
		public override bool IsColorless => false;

		private static Color LinearToSrgbApprox(Color linear)
		{
			const float gamma = 2.2f;
			return new Color(
				Mathf.Pow(linear.R, 1f / gamma),
				Mathf.Pow(linear.G, 1f / gamma),
				Mathf.Pow(linear.B, 1f / gamma),
				linear.A
			);
		}
	}
}
