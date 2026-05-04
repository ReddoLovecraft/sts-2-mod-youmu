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
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.ValueProps;
using Patchoulib.Scrpits.Main;
using TH_Youmu.Scripts.Main;
using TH_Youmu.Scrpits.Powers;

namespace TH_Youmu.Scrpits.Cards
{
[Pool(typeof(YoumuCardPool))]
public class HeartEyeLostSlash : YoumuCardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(6, ValueProp.Move)];
	
	protected override IEnumerable<IHoverTip> ExtraHoverTips => (new IHoverTip[1]
    {
	 	Tools.GetStaticKeyword("Derive")
    });
	
	public HeartEyeLostSlash() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
	{
	}
	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
			.WithHitFx("vfx/vfx_attack_slash")
			.Execute(choiceContext);
	 	await ToolBox.Derive(choiceContext,Owner,CardType.Attack,1);
	}
	 public override async Task TriggerWhenDerive(PlayerChoiceContext choiceContext,Player player,CardType cardType,int amount,bool IsAny=false)
    {
		IReadOnlyList<CardModel> cards = PileType.Hand.GetPile(base.Owner).Cards;
		Rng combatCardSelection = base.Owner.RunState.Rng.CombatCardSelection;
		CardModel cardModel = combatCardSelection.NextItem(cards.Where((CardModel c) => c.CostsEnergyOrStars(includeGlobalModifiers: false)));
		if (cardModel == null)
		{
			combatCardSelection.NextItem(cards.Where((CardModel c) => c.CostsEnergyOrStars(includeGlobalModifiers: true)));
		}
		cardModel?.SetToFreeThisTurn();
    }
	protected override void OnUpgrade()
	{
		this.DynamicVars.Damage.UpgradeValueBy(3);
	}
}

}
