using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Runs;
using TH_Youmu.Scrpits.Main;

namespace TH_Youmu.Scrpits.Events;

public sealed class HungryYuyuko : YoumuEventModel
{
	protected override IEnumerable<DynamicVar> CanonicalVars =>
	[
		new StringVar("FoodRelics"),
		new IntVar("FoodRelicCounts", 0m)
	];
	public override string? CustomInitialPortraitPath => "res://TH_Youmu/ArtWorks/Events/hungryyuyuko.png";
	public override bool IsAllowed(IRunState runState)
	{
		return HasAllYoumu(runState);
	}

	public override void CalculateVars()
	{
		base.CalculateVars();

		List<RelicModel> foodRelics = GetFoodRelics(Owner!);
		((StringVar)DynamicVars["FoodRelics"]).StringValue = JoinRelicTitles(foodRelics);
		DynamicVars["FoodRelicCounts"].BaseValue = foodRelics.Count;
	}

	protected override IReadOnlyList<EventOption> GenerateInitialOptions()
	{
		List<RelicModel> foodRelics = GetFoodRelics(Owner!);
		IReadOnlyList<IHoverTip> foodRelicHoverTips = foodRelics.SelectMany(HoverTipFactory.FromRelic).ToList();

		EventOption feedOption = foodRelics.Count > 0
			? CreateOption(Feed, "TH_YOUMU-HUNGRY_YUYUKO.pages.INITIAL.options.FEED", foodRelicHoverTips)
			: CreateOption(null, "TH_YOUMU-HUNGRY_YUYUKO.pages.INITIAL.options.FEED_LOCKED", Array.Empty<IHoverTip>());

		EventOption faceOption = CreateOption(Face, "TH_YOUMU-HUNGRY_YUYUKO.pages.INITIAL.options.FACE");

		return [feedOption, faceOption];
	}

	private static List<RelicModel> GetFoodRelics(MegaCrit.Sts2.Core.Entities.Players.Player player)
	{
		return player.Relics.Where(IsFoodRelic).ToList();
	}

	private static bool IsFoodRelic(RelicModel relic)
	{
		return relic is Strawberry
			|| relic is Mango
			|| relic is Pear
			|| relic is DragonFruit
			|| relic is LeesWaffle
			|| relic is NutritiousOyster
			|| relic is LoomingFruit
			|| relic is BigMushroom
			|| relic is ChosenCheese
			|| relic is FakeMango;
	}

	private static string JoinRelicTitles(IEnumerable<RelicModel> relics)
	{
		List<string> names = relics.Select(r => r.Title.GetFormattedText()).ToList();
		if (names.Count == 0)
		{
			return "无";
		}
		return string.Join("、", names);
	}

	private async Task Feed()
	{
		List<RelicModel> foodRelics = GetFoodRelics(Owner!);
		int count = foodRelics.Count;
		foreach (RelicModel relic in foodRelics)
		{
			await RelicCmd.Remove(relic);
		}

		if (count > 0)
		{
			CardSelectorPrefs prefs = new CardSelectorPrefs(CardSelectorPrefs.RemoveSelectionPrompt, 0, count)
			{
				Cancelable = true
			};
			List<CardModel> cards = (await CardSelectCmd.FromDeckForRemoval(prefs: prefs, player: Owner!)).ToList();
			if (cards.Count > 0)
			{
				await CardPileCmd.RemoveFromDeck(cards);
			}
		}

		SetEventFinished(PageDescription("FEED"));
	}

	private async Task Face()
	{
		await CreatureCmd.LoseMaxHp(new ThrowingPlayerChoiceContext(), Owner!.Creature, 35, false);
		await PlayerCmd.GainGold(600, Owner!);
		SetEventFinished(PageDescription("FACE"));
	}
}
