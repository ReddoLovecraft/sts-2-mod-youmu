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
public class MeditationSlash : YoumuCardModel
{
   
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(21, ValueProp.Move)];
	protected override IEnumerable<IHoverTip> ExtraHoverTips => (new IHoverTip[1]
    {
       HoverTipFactory.FromPower<StiffnessPower>()
    });
	public MeditationSlash() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
	{
	}
	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
		.WithHitFx("vfx/vfx_attack_slash", null, "slash_attack.mp3").Execute(choiceContext);;
		if(!Owner.HasPower<StiffnessPower>())
		(await PowerCmd.Apply<StiffnessPower>(base.Owner.Creature,1,Owner.Creature,this)).SetStiffType(StiffType.SpellCard);
	}
	protected override void OnUpgrade()
	{
		DynamicVars.Damage.UpgradeValueBy(6);
	}
}

}
