using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
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
public class CutSoulSword : YoumuCardModel
{
	public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
	protected override bool ShouldGlowGoldInternal => ToolBox.WasLastCardPlayedSpecificCard(Owner,this,CardType.Skill);
	protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(1, ValueProp.Move), new EnergyVar(1)];
	
	protected override IEnumerable<IHoverTip> ExtraHoverTips => (new IHoverTip[1]
    {
	 	HoverTipFactory.FromPower<StiffnessPower>()
    });
	
	public CutSoulSword() : base(1, CardType.Attack, CardRarity.Rare, TargetType.AnyAlly)
	{
	}
	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		if (cardPlay.Target == null)
		{
			await Task.CompletedTask;
			return;
		}

		Player? targetPlayer = cardPlay.Target.Player;
		if (targetPlayer == null)
		{
			await Task.CompletedTask;
			return;
		}

		decimal damage = base.DynamicVars.Damage.BaseValue;

		List<CardModel> statusAndCurses = targetPlayer.PlayerCombatState.AllCards
			.Where(c => (c.Type == CardType.Status || c.Type == CardType.Curse) && c.Pile?.Type != PileType.Exhaust)
			.ToList();

		foreach (CardModel c in statusAndCurses)
		{
			await CardCmd.Exhaust(choiceContext, c);
			damage += 1m;
			await CardPileCmd.Draw(choiceContext, 1, base.Owner);
			await CardPileCmd.Draw(choiceContext, 1, targetPlayer);
		}

		List<PowerModel> debuffs = cardPlay.Target.Powers.Where(p => p.Type == PowerType.Debuff).ToList();
		for (int i = debuffs.Count - 1; i >= 0; i--)
		{
			await PowerCmd.Remove(debuffs[i]);
			damage *= 2m;
			await PlayerCmd.GainEnergy(base.DynamicVars.Energy.IntValue, base.Owner);
			await PlayerCmd.GainEnergy(base.DynamicVars.Energy.IntValue, targetPlayer);
		}

		await DamageCmd.Attack(damage).FromCard(this).Targeting(cardPlay.Target)
			.WithHitFx("vfx/vfx_attack_slash", null, "slash_attack.mp3")
			.Execute(choiceContext);
	}
	protected override void OnUpgrade()
	{
		this.AddKeyword(CardKeyword.Retain);
	}
}

}
