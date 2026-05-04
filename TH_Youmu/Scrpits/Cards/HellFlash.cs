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
using TH_Youmu.Scripts.Main;
using TH_Youmu.Scrpits.Powers;

namespace TH_Youmu.Scrpits.Cards
{
[Pool(typeof(YoumuCardPool))]
public class HellFlash : YoumuCardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new EnergyVar(1),new CardsVar(1)];
		protected override IEnumerable<IHoverTip> ExtraHoverTips => (new IHoverTip[1]
    {
	 	base.EnergyHoverTip
    });
	protected override bool ShouldGlowGoldInternal =>GetStatusOrCurseInHand(base.Owner).Count()>0;
	public HellFlash() : base(0, CardType.Skill, CardRarity.Rare, TargetType.None)
	{
	}
	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);
		await PlayerCmd.GainEnergy(this.DynamicVars.Energy.IntValue,base.Owner);
		await CardPileCmd.Draw(choiceContext,this.DynamicVars.Cards.IntValue,Owner);
		if(ShouldGlowGoldInternal)
		{
			await PlayerCmd.GainEnergy(this.DynamicVars.Energy.IntValue,base.Owner);
			await CardPileCmd.Draw(choiceContext,this.DynamicVars.Cards.IntValue,Owner);
		}
	}
	private static IEnumerable<CardModel> GetStatusOrCurseInHand(Player owner)
	{
		return owner.PlayerCombatState.AllCards.Where((CardModel c) => (c.Type==CardType.Status||c.Type==CardType.Curse) && (c.Pile.Type==PileType.Hand));
	}
	protected override void OnUpgrade()
	{
		this.DynamicVars.Energy.UpgradeValueBy(1);
		this.DynamicVars.Cards.UpgradeValueBy(1);
	}
}

}
