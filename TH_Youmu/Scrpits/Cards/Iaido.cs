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
public class Iaido : YoumuCardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(10, ValueProp.Move)];
	protected override bool ShouldGlowGoldInternal => ToolBox.WasLastCardPlayedSpecificCard(Owner,this,CardType.Skill);
	public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Retain];
	
	protected override IEnumerable<IHoverTip> ExtraHoverTips => (new IHoverTip[1]
    {
	 	HoverTipFactory.FromPower<StiffnessPower>()
    });
	
	public Iaido() : base(1, CardType.Attack, CardRarity.Common, TargetType.AllEnemies)
	{
	}
	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		int muti=ShouldGlowGoldInternal?2:1;
		await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue*muti).FromCard(this).TargetingAllOpponents(base.CombatState)
			.WithHitFx("vfx/vfx_attack_slash")
			.Execute(choiceContext);
		if(!Owner.HasPower<StiffnessPower>())
		(await PowerCmd.Apply<StiffnessPower>(Owner.Creature,3,Owner.Creature,this)).SetStiffType(StiffType.None);
	}
	protected override void OnUpgrade()
	{
		this.DynamicVars.Damage.UpgradeValueBy(3);
	}
}

}
