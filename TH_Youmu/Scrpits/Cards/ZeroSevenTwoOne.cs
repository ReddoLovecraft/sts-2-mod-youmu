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
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;
using Patchoulib.Scrpits.Main;
using TH_Youmu.Scripts.Main;
using TH_Youmu.Scrpits.Powers;

namespace TH_Youmu.Scrpits.Cards
{
[Pool(typeof(YoumuCardPool))]
public class ZeroSevenTwoOne : YoumuCardModel
{
	 public override int MaxUpgradeLevel =>3;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(5, ValueProp.Move),new CardsVar(2),new DynamicVar("Power",2)];
	protected override IEnumerable<IHoverTip> ExtraHoverTips => (new IHoverTip[2]
    {
		HoverTipFactory.FromPower<SwordGasPower>(),
	 	Tools.GetStaticKeyword("Cancel")  
    });
	public override CancelType CancelLevel=>CancelType.Final;
	public ZeroSevenTwoOne() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
	{
	}
	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).FromCard(this).WithHitCount(this.DynamicVars.Cards.IntValue).Targeting(cardPlay.Target)
			.WithHitFx("vfx/vfx_attack_slash")
			.Execute(choiceContext);
		await PowerCmd.Apply<SwordGasPower>(Owner.Creature,this.DynamicVars["Power"].IntValue,Owner.Creature,this);
		await ToolBox.Cancel(choiceContext,Owner,this);
	}
	protected override void OnUpgrade()
	{
		this.DynamicVars.Damage.UpgradeValueBy(1);
		this.DynamicVars["Power"].UpgradeValueBy(2);
	}
}

}
