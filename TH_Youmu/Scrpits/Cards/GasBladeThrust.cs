using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;
using Patchoulib.Scrpits.Main;
using TH_Youmu.Scripts.Main;
using TH_Youmu.Scrpits.Powers;

namespace TH_Youmu.Scrpits.Cards
{
[Pool(typeof(YoumuCardPool))]
public class GasBladeThrust : YoumuCardModel
{
   
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(10, ValueProp.Move),new CardsVar(2)];
		protected override IEnumerable<IHoverTip> ExtraHoverTips => (new IHoverTip[3]
    {
		HoverTipFactory.FromPower<SwordGasPower>(),
		HoverTipFactory.FromCard<HorizontalSlash>(base.IsUpgraded),
		Tools.GetStaticKeyword("Derive")
    });
	public GasBladeThrust() : base(2, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
	{
	}
	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		SfxCmd.Play("event:/sfx/characters/ironclad/ironclad_whirlwind");
		await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).WithHitCount(1).FromCard(this)
			.Targeting(cardPlay.Target)
			.WithHitVfxNode((Creature t) => NStabVfx.Create(t, facingEnemies: true, VfxColor.Red))
			.Execute(choiceContext);
		await PowerCmd.Apply<SwordGasPower>(Owner.Creature,this.DynamicVars.Cards.IntValue,Owner.Creature,this);
		CardModel dl = base.CombatState.CreateCard<HorizontalSlash>(base.Owner);
		if(base.IsUpgraded)
		{
			CardCmd.Upgrade(dl);
		}
		CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardToCombat(dl, PileType.Draw, addedByPlayer: true), 0.8f);
		await ToolBox.Derive(choiceContext,Owner,CardType.Attack,1);
	}
	protected override void OnUpgrade()
	{
		DynamicVars.Damage.UpgradeValueBy(3); 
		DynamicVars.Cards.UpgradeValueBy(1);
	}
}

}
