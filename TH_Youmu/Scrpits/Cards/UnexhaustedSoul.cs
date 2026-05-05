using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
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
public class UnexhaustedSoul : YoumuCardModel
{
	private const string _exhaustReductionKey = "ExhaustReduction";

	public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Ethereal];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(11, ValueProp.Move), new IntVar(_exhaustReductionKey, 0m)];
	protected override IEnumerable<IHoverTip> ExtraHoverTips => (new IHoverTip[1]
    {
	 	HoverTipFactory.FromKeyword(CardKeyword.Exhaust)
    });
	public UnexhaustedSoul() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
	{
	}

	private void RefreshExhaustReduction()
	{
		Player? owner = Owner;
		if (owner == null)
		{
			return;
		}

		int count = PileType.Exhaust.GetPile(owner).Cards.Count;
		base.DynamicVars[_exhaustReductionKey].BaseValue = count;
	}

	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		RefreshExhaustReduction();
		List<CardModel> list = PileType.Exhaust.GetPile(base.Owner).Cards.ToList();
		int cardCount = list.Count;
		decimal line=base.DynamicVars.Damage.BaseValue;
		AttackCommand attackCommand =await DamageCmd.Attack(line-cardCount).FromCard(this).Targeting(cardPlay.Target)
			.WithHitFx("vfx/vfx_starry_impact")
			.Execute(choiceContext);
		if(attackCommand.Results.Sum((DamageResult r) => r.TotalDamage + r.OverkillDamage) >= line)
		{
			CardSelectorPrefs prefs = new CardSelectorPrefs(base.SelectionScreenPrompt, 1);
			List<CardModel> cardsIn = PileType.Exhaust.GetPile(base.Owner).Cards.ToList();
			if(cardsIn.Count>0)
			foreach(CardModel card in await CardSelectCmd.FromSimpleGrid(choiceContext, cardsIn, base.Owner, prefs))
			{
				if(card!=null)
				await CardPileCmd.Add(card, PileType.Hand);
			}
		}
	}
	protected override void OnUpgrade()
	{
		this.DynamicVars.Damage.UpgradeValueBy(4);
	}

	public override async Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, CombatState combatState)
	{
		RefreshExhaustReduction();
		await Task.CompletedTask;
	}

	public override async Task AfterCardChangedPiles(CardModel card, PileType oldPileType, AbstractModel? source)
	{
		if (Owner != null && card.Owner == Owner)
		{
			RefreshExhaustReduction();
		}

		await Task.CompletedTask;
	}
}

}
