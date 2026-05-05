using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using Patchoulib.Scrpits.Main;
using TH_Youmu.Scripts.Main;
using TH_Youmu.Scrpits.Powers;

namespace TH_Youmu.Scrpits.Cards
{
[Pool(typeof(YoumuCardPool))]
public class LearnNoStop : YoumuCardModel
{
	private const string _increaseKey = "Increase";

	private int _currentCard = 1;

	private int _increasedCard;

	[SavedProperty]
	public int CurrentCard
	{
		get
		{
			return _currentCard;
		}
		set
		{
			AssertMutable();
			_currentCard = value;
			base.DynamicVars.Cards.BaseValue = _currentCard;
		}
	}
    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(0),new IntVar("Increase", 1m)];
	[SavedProperty]
	public int IncreasedCard
	{
		get
		{
			return _increasedCard;
		}
		set
		{
			AssertMutable();
			_increasedCard = value;
		}
	}
	public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust,CardKeyword.Innate,CardKeyword.Eternal];
	public LearnNoStop() : base(2, CardType.Skill, CardRarity.Rare, TargetType.Self)
	{
	}
	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		IEnumerable<CardModel> enumerable = PileType.Deck.GetPile(base.Owner).Cards.Where((CardModel c) => c?.IsUpgradable ?? false).ToList().StableShuffle(base.Owner.RunState.Rng.Niche)
		.Take(this.DynamicVars.Cards.IntValue);
		{
			foreach (CardModel item in enumerable)
			{
					CardCmd.Upgrade(item);
			}
		}
		int intValue = base.DynamicVars["Increase"].IntValue;
		BuffFromPlay(intValue);
		(base.DeckVersion as LearnNoStop)?.BuffFromPlay(intValue);
	}
	public override bool ShouldPlay(CardModel card, AutoPlayType autoPlayType)
	{
		if (card.Owner != base.Owner)
		{
			return true;
		}
		CardPile? pile = base.Pile;
		if (pile == null || pile.Type != PileType.Hand)
		{
			return true;
		}
		if (card is LearnNoStop)
		{
			return true;
		}
		if (autoPlayType != AutoPlayType.None)
		{
			return true;
		}
		return false;
	}
	protected override void OnUpgrade()
	{
	  this.EnergyCost.UpgradeBy(-1);
	}
	protected override void AfterDowngraded()
	{
		UpdateCard();
	}

	private void BuffFromPlay(int extraCard)
	{
		IncreasedCard += extraCard;
		UpdateCard();
	}

	private void UpdateCard()
	{
		CurrentCard = 0 + IncreasedCard;
	}
}

}
