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
public class GhostSwordGod : YoumuCardModel
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Ethereal];
	protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(50)];
	public GhostSwordGod() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
	{
	}
	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);
		await PowerCmd.Apply<GhostSwordGodPower>(base.Owner.Creature,base.DynamicVars.Cards.IntValue,base.Owner.Creature,this);
	}
	protected override void OnUpgrade()
	{
	    this.DynamicVars.Cards.UpgradeValueBy(25);
	}
}

}
