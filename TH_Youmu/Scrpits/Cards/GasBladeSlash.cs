using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
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
public class GasBladeSlash : YoumuCardModel
{
	protected override IEnumerable<IHoverTip> ExtraHoverTips => (new IHoverTip[3]
    {
        Tools.GetStaticKeyword("Derive"),
		HoverTipFactory.FromPower<SwordGasPower>(),
		HoverTipFactory.FromCard<GasBladeGreatSpin>()
	    });
   
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(7, ValueProp.Move),new CardsVar(1)];
	public GasBladeSlash() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
	{
	}
	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
		.WithHitFx("vfx/vfx_attack_slash", null, "slash_attack.mp3").Execute(choiceContext);;
		await PowerCmd.Apply<SwordGasPower>(Owner.Creature, 1,Owner.Creature,this);
	}
	 public override async Task TriggerWhenDerive(PlayerChoiceContext choiceContext,Player player,CardType cardType,int amount,bool IsAny=false)
    {
       if(Owner.HasPower<SwordGasPower>()&&Owner.Creature.GetPowerAmount<SwordGasPower>()>=3)
       {	
		  await PowerCmd.ModifyAmount(Owner.Creature.GetPower<SwordGasPower>(),-3,null,null);
		  await CardPileCmd.AddToCombatAndPreview<GasBladeGreatSpin>(Owner.Creature, PileType.Hand, 1, addedByPlayer: true);
       }
       else
       {
          this.EnergyCost.SetThisTurnOrUntilPlayed(0);
       }
    }
	protected override void OnUpgrade()
	{
		DynamicVars.Damage.UpgradeValueBy(3); 
	}
}

}
