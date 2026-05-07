using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using Patchouib.Scrpits.Main;
using Patchoulib.Scrpits.Main;
using TH_Youmu.Scripts.Main;
using TH_Youmu.Scrpits.Powers;

namespace TH_Youmu.Relics
{
[Pool(typeof(YoumuRelicPool))]
public class HalfSoulTailsman : CustomRelicModel
{
	public override string PackedIconPath => $"res://TH_Youmu/ArtWorks/Relics/{Id.Entry}.png";
    protected override string PackedIconOutlinePath => $"res://TH_Youmu/ArtWorks/Relics/Outlines/{Id.Entry}.png";
    protected override string BigIconPath => $"res://TH_Youmu/ArtWorks/Relics/{Id.Entry}.png";
    public override RelicRarity Rarity => RelicRarity.Rare;
	bool flag=true;
   	public override Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, CombatState combatState)
	{
		if (side != base.Owner.Creature.Side)
		{
			return Task.CompletedTask;
		}
		base.Status = RelicStatus.Active;
		flag=true;
		return Task.CompletedTask;
	}
		public override bool TryModifyPowerAmountReceived(PowerModel canonicalPower, Creature target, decimal amount, Creature? _, out decimal modifiedAmount)
	{
		if (target != base.Owner.Creature)
		{
			modifiedAmount = amount;
			return false;
		}
		if (canonicalPower.GetTypeForAmount(amount) != PowerType.Debuff)
		{
			modifiedAmount = amount;
			return false;
		}
		if (!canonicalPower.IsVisible)
		{
			modifiedAmount = amount;
			return false;
		}
		if (!flag)
		{
			modifiedAmount = amount;
			return false;
		}
		flag=false;
		this.Flash();
		base.Status = RelicStatus.Normal;
		modifiedAmount = default(decimal);
		return true;
	}
	public override Task AfterCombatEnd(CombatRoom room)
	{
		base.Status = RelicStatus.Normal;
		return Task.CompletedTask;
	}
}
}
