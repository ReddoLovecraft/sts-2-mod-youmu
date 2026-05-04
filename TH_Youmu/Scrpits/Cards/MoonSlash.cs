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
public class MoonSlash : YoumuCardModel
{
	 public override int MaxUpgradeLevel =>3;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(9, ValueProp.Move),new CardsVar(2),new DynamicVar("Power",3)];
	protected override IEnumerable<IHoverTip> ExtraHoverTips => (new IHoverTip[2]
    {
	 	Tools.GetStaticKeyword("Cancel") ,
		HoverTipFactory.FromPower<StiffnessPower>()
    });
	public override CancelType CancelLevel=>CancelType.Final;
	protected override bool ShouldGlowGoldInternal => base.CombatState?.HittableEnemies.Any((Creature e) => e.Monster.IntendsToAttack) ?? false;
	public MoonSlash() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
	{
	}
	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		int addtionMuti=1;
		if(cardPlay.Target!=null&&cardPlay.Target.Monster.IntendsToAttack)
		{
			addtionMuti=this.DynamicVars.Cards.IntValue;
		}
		await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue*addtionMuti).FromCard(this).Targeting(cardPlay.Target)
			.WithHitFx("vfx/vfx_attack_slash")
			.Execute(choiceContext);
		await ToolBox.Cancel(choiceContext,Owner,this);
		if(!Owner.HasPower<StiffnessPower>())
		{
			(await PowerCmd.Apply<StiffnessPower>(Owner.Creature,this.DynamicVars["Power"].IntValue,Owner.Creature,this)).SetStiffType(StiffType.Final);
		}
	}
	protected override void OnUpgrade()
	{
		this.DynamicVars.Damage.UpgradeValueBy(2);
		this.DynamicVars["Power"].UpgradeValueBy(-1);
		this.DynamicVars.Cards.UpgradeValueBy(1);
	}
}

}
