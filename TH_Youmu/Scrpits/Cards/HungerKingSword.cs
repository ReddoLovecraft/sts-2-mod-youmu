using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Events;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;
using Patchoulib.Scrpits.Main;
using System.Collections.Generic;
using System.Linq;
using TH_Youmu.Scripts.Main;
using TH_Youmu.Scrpits.Powers;

namespace TH_Youmu.Scrpits.Cards
{
[Pool(typeof(YoumuCardPool))]
public class HungerKingSword : YoumuCardModel
{
	public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(10, ValueProp.Move)];
	public HungerKingSword() : base(2, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies)
	{
	}
	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		List<CardModel> toConsume = PileType.Draw.GetPile(base.Owner).Cards
			.Where(c => c.Type == CardType.Curse || c.Type == CardType.Status)
			.ToList();

		foreach (CardModel c in toConsume)
		{
			await CardCmd.Exhaust(choiceContext, c);
			await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).FromCard(this)
				.WithHitFx("vfx/vfx_attack_slash", null, "slash_attack.mp3")
				.TargetingAllOpponents(base.CombatState)
				.SpawningHitVfxOnEachCreature()
				.Execute(choiceContext);
		}
	}
	protected override void OnUpgrade()
	{
		this.EnergyCost.UpgradeBy(-1);
	}
	public override async Task AfterCardChangedPiles(CardModel card, PileType oldPileType, AbstractModel? source)
	{
		if (!IsInCombat)
		{
			await Task.CompletedTask;
			return;
		}
		if (card == this)
		{
			await Task.CompletedTask;
			return;
		}

		if (card.Owner != base.Owner)
		{
			await Task.CompletedTask;
			return;
		}

		if (card.Pile?.Type != PileType.Draw)
		{
			await Task.CompletedTask;
			return;
		}

		if (oldPileType == PileType.Draw)
		{
			await Task.CompletedTask;
			return;
		}

		if (card.Type != CardType.Curse && card.Type != CardType.Status)
		{
			await Task.CompletedTask;
			return;
		}

		CardPile? myPile = base.Pile;
		if (myPile != null && myPile.Type == PileType.Hand)
		{
			await Task.CompletedTask;
			return;
		}
		await CardPileCmd.Add(this, PileType.Hand);
	}
}

}
