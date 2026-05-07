using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
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
public class GhostMooncake : CustomRelicModel
{
	public override string PackedIconPath => $"res://TH_Youmu/ArtWorks/Relics/{Id.Entry}.png";
    protected override string PackedIconOutlinePath => $"res://TH_Youmu/ArtWorks/Relics/Outlines/{Id.Entry}.png";
    protected override string BigIconPath => $"res://TH_Youmu/ArtWorks/Relics/{Id.Entry}.png";
    public override RelicRarity Rarity => RelicRarity.Rare;
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<IntangiblePower>(),HoverTipFactory.FromKeyword(CardKeyword.Ethereal)];

	private bool _shouldGrantIntangible;
	public override async Task BeforeTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
	{
		if (side == base.Owner.Creature.Side && _shouldGrantIntangible)
		{
			Flash();
			await PowerCmd.Apply<IntangiblePower>(Owner.Creature,1,Owner.Creature,null);
		}
		_shouldGrantIntangible = false;
		base.Status = RelicStatus.Normal;
	}

	public override Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
	{
		if (!cardPlay.Card.Keywords.Contains(CardKeyword.Ethereal))
		{
			return Task.CompletedTask;
		}
		if (cardPlay.Card.Owner != base.Owner)
		{
			return Task.CompletedTask;
		}
		_shouldGrantIntangible = true;
		base.Status = RelicStatus.Active;
		return Task.CompletedTask;
	}

	public override async Task AfterSideTurnStart(CombatSide side, CombatState combatState)
	{
		if (side == base.Owner.Creature.Side)
		{
			_shouldGrantIntangible = false;
			base.Status = RelicStatus.Normal;
			return;
		}
		if (!_shouldGrantIntangible)
		{
			return;
		}
		Flash();
		await PowerCmd.Apply<IntangiblePower>(Owner.Creature, 1, Owner.Creature, null);
		_shouldGrantIntangible = false;
		base.Status = RelicStatus.Normal;
	}

	public override Task AfterCombatEnd(CombatRoom room)
	{
		_shouldGrantIntangible = false;
		base.Status = RelicStatus.Normal;
		return Task.CompletedTask;
	}

}
}
