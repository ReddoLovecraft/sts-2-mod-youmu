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
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
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
public class GhostSword : YoumuCardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(8, ValueProp.Move),new CardsVar(1)];
	protected override IEnumerable<IHoverTip> ExtraHoverTips => (new IHoverTip[3]
    {
	 	HoverTipFactory.FromPower<WeakPower>(),
	 	HoverTipFactory.FromPower<VulnerablePower>(),
	 	HoverTipFactory.FromCard<Decay>(),		
       });
	public GhostSword() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
	{
	}
	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
			.WithHitFx("vfx/vfx_attack_slash", null, "slash_attack.mp3")
			.Execute(choiceContext);
		if(cardPlay.Target!=null&&cardPlay.Target.IsAlive)
		{
			await PowerCmd.Apply<WeakPower>(cardPlay.Target,this.DynamicVars.Cards.IntValue,Owner.Creature,this);
			await PowerCmd.Apply<VulnerablePower>(cardPlay.Target,this.DynamicVars.Cards.IntValue,Owner.Creature,this);
		}
        CardModel card = base.CombatState.CreateCard<Decay>(base.Owner);
		CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Draw, addedByPlayer: true));
	}
	protected override void OnUpgrade()
	{
		this.DynamicVars.Damage.UpgradeValueBy(1);
		this.DynamicVars.Cards.UpgradeValueBy(1);
	}
}

}
