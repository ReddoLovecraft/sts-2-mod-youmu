using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Patchoulib.Scrpits.Main;
using TH_Youmu.Scripts.Main;
using TH_Youmu.Scrpits.Powers;

namespace TH_Youmu.Scrpits.Cards
{
[Pool(typeof(YoumuCardPool))]
public class Foresight : YoumuCardModel
{
   
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(9, ValueProp.Move)];
	protected override IEnumerable<IHoverTip> ExtraHoverTips => (new IHoverTip[4]
    {
        HoverTipFactory.FromPower<SwordGasPower>(),
        Tools.GetStaticKeyword("OpenSword"),
        Tools.GetStaticKeyword("Sword"),
		Tools.GetStaticKeyword("Derive")
    });
	public Foresight() : base(1, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy)
	{
	}
	protected override bool ShouldGlowGoldInternal => base.CombatState?.HittableEnemies.Any((Creature e) => e.Monster.IntendsToAttack) ?? false;
	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		SfxCmd.Play("event:/sfx/characters/ironclad/ironclad_whirlwind");
		await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this)
			.Targeting(cardPlay.Target)
			.WithHitFx("vfx/vfx_giant_horizontal_slash")
			.Execute(choiceContext);
		if(Owner.HasPower<SwordGasPower>())
		{
			await PowerCmd.Remove(Owner.Creature.GetPower<SwordGasPower>());
		}
		if (cardPlay.Target?.Monster != null && cardPlay.Target.Monster.IntendsToAttack)
        {
            await ToolBox.OpenSword(Owner,this);
        }
		await ToolBox.Derive(choiceContext,Owner,CardType.Attack,1);
	}
	protected override void OnUpgrade()
	{
		DynamicVars.Damage.UpgradeValueBy(3); 
	}
}

}
