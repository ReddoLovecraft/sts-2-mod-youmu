using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Patchoulib.Scrpits.Main;
using TH_Youmu.Scripts.Main;
using TH_Youmu.Scrpits.Powers;

namespace TH_Youmu.Scrpits.Cards
{
[Pool(typeof(YoumuCardPool))]
public class TruthEternal : YoumuCardModel
{
	public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Eternal];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(9, ValueProp.Move)];

	public TruthEternal() : base(1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
	{
	}
	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
		.WithHitFx("vfx/vfx_attack_slash", null, "slash_attack.mp3").Execute(choiceContext);;
		foreach (CardModel item in PileType.Hand.GetPile(base.Owner).Cards.ToList())
		{
			if(item.IsUpgradable)
			CardCmd.Upgrade(item);
			else if(item.IsUpgraded)
			{
				item.EnergyCost.AddThisCombat(-1);
			}
		}
	}
	public override async Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, CombatState combatState)
	{
		if (player == base.Owner)
		{
			CardPile? pile = base.Pile;
			if (pile == null || pile.Type != PileType.Hand)
			{
				await CardPileCmd.Add(this, PileType.Hand);
			}
		}
	}
	protected override void OnUpgrade()
	{
		DynamicVars.Damage.UpgradeValueBy(4); 
	}
}

}
