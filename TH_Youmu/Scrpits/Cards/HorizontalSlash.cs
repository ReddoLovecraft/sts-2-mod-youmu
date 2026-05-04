using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.ValueProps;
using Patchoulib.Scrpits.Main;
using TH_Youmu.Scripts.Main;
using TH_Youmu.Scrpits.Powers;

namespace TH_Youmu.Scrpits.Cards
{
[Pool(typeof(StatusCardPool))]
public class HorizontalSlash : YoumuCardModel
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust,CardKeyword.Ethereal];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(6, ValueProp.Move)];
	protected override IEnumerable<IHoverTip> ExtraHoverTips => (new IHoverTip[2]
    {
        Tools.GetStaticKeyword("Sword"),
		Tools.GetStaticKeyword("Derive")
    });
	public HorizontalSlash() : base(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
	{
	}
	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
			.WithHitFx("vfx/vfx_heavy_blunt")
			.Execute(choiceContext);
		if(Owner.HasPower<RedSwordPower>())
		{
			await PowerCmd.Remove(Owner.Creature.GetPower<RedSwordPower>());
			await PowerCmd.Apply<YellowSwordPower>(Owner.Creature,3,Owner.Creature,this);
		}
		else if(Owner.HasPower<YellowSwordPower>())
		{
			await PowerCmd.Remove(Owner.Creature.GetPower<YellowSwordPower>());
			await PowerCmd.Apply<WhiteSwordPower>(Owner.Creature,3,Owner.Creature,this);
		}
		else if(Owner.HasPower<WhiteSwordPower>())
		{
			await PowerCmd.Remove(Owner.Creature.GetPower<WhiteSwordPower>());
		}
	}
	public override async Task TriggerWhenDerive(PlayerChoiceContext choiceContext,Player player,CardType cardType,int amount,bool IsAny=false)
    {
		int muti=1;
		if(Owner.HasPower<WhiteSwordPower>())
		{
			muti=2;
		}
		else if(Owner.HasPower<YellowSwordPower>())
		{
			muti=4;
		}
		else if(Owner.HasPower<RedSwordPower>())
		{
			muti=8;
		}
        this.DynamicVars.Damage.UpgradeValueBy(this.DynamicVars.Damage.BaseValue*muti);
    }
	protected override void OnUpgrade()
	{
		DynamicVars.Damage.UpgradeValueBy(3); 
	}
}

}
