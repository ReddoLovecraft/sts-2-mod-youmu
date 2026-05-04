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
public class SwordDefend : YoumuCardModel
{
	public override bool GainsBlock => true;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(2, ValueProp.Move),new CardsVar(3)];
	protected override IEnumerable<IHoverTip> ExtraHoverTips => (new IHoverTip[1]
    {
	 	HoverTipFactory.FromPower<SwordGasPower>()
    });
	public SwordDefend() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
	{
	}
	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		int amount =0;
		if(Owner.HasPower<SwordGasPower>())
		amount =Owner.Creature.GetPowerAmount<SwordGasPower>();
		if(Owner.Character is YoumuCharacter)
		await CreatureCmd.TriggerAnim(base.Owner.Creature, "Guard", base.Owner.Character.CastAnimDelay);
		for(int i=0;i<this.DynamicVars.Cards.IntValue;i++)
		await CreatureCmd.GainBlock(Owner.Creature,this.DynamicVars.Block.IntValue+amount,ValueProp.Move,cardPlay);
	}
	protected override void OnUpgrade()
	{
		this.DynamicVars.Block.UpgradeValueBy(1);
	}
}

}
