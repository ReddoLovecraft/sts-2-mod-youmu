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
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;
using Patchoulib.Scrpits.Main;
using TH_Youmu.Scripts.Main;
using TH_Youmu.Scrpits.Powers;

namespace TH_Youmu.Scrpits.Cards
{
[Pool(typeof(YoumuCardPool))]
public class CardSlasher : YoumuCardModel
{
	protected override IEnumerable<IHoverTip> ExtraHoverTips => (new IHoverTip[2]
    {
		base.EnergyHoverTip,
		HoverTipFactory.FromKeyword(CardKeyword.Exhaust)
    });
	  protected override IEnumerable<DynamicVar> CanonicalVars => [new  EnergyVar(1)];
	public CardSlasher() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
	{
	}
	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);
		await PowerCmd.Apply<CardSlasherPower>(base.Owner.Creature,1,base.Owner.Creature,this);
	}
	protected override void OnUpgrade()
	{
		this.AddKeyword(CardKeyword.Innate);
	}
}

}
