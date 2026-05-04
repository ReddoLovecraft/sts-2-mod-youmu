using MegaCrit.Sts2.Core.Models;
using Patchouib.Scripts.Main;
using TH_Youmu.Scrpits.Cards;

namespace TH_Youmu.Scrpits.Powers
{
public sealed class RiseUpSlashPower :CustomTempStrengthPower
{
	public override AbstractModel OriginModel => ModelDb.Card<RiseUpSlash>();
    protected override bool IsPositive => false;
    public override string? CustomPackedIconPath => "res://TH_Youmu/ArtWorks/Powers/RSP232.png";
    public override string? CustomBigIconPath => "res://TH_Youmu/ArtWorks/Powers/RSP264.png";
      
}

}