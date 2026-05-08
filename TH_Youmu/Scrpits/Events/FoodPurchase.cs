using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Gold;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Runs;
using TH_Youmu.Scrpits.Main;

namespace TH_Youmu.Scrpits.Events;

public sealed class FoodPurchase : YoumuEventModel
{
	private const int OneCost = 150;
	private const int ThreeCost = 300;
	private const int AllCost = 600;
	public override string? CustomInitialPortraitPath => "res://TH_Youmu/ArtWorks/Events/foodpurchase.png";
	public override bool IsAllowed(IRunState runState)
	{
		return HasAnyYoumu(runState) && runState.Players.All(p => p.Gold >= OneCost);
	}

	protected override IReadOnlyList<EventOption> GenerateInitialOptions()
	{
		bool canAffordOne = CanAllPlayersAfford(OneCost);
		bool canAffordThree = CanAllPlayersAfford(ThreeCost);
		bool canAffordAll = CanAllPlayersAfford(AllCost);

		IReadOnlyList<IHoverTip> randomFoodRelicTips =
		[
			.. HoverTipFactory.FromRelic<Strawberry>(),
			.. HoverTipFactory.FromRelic<Mango>(),
			.. HoverTipFactory.FromRelic<Pear>(),
			.. HoverTipFactory.FromRelic<DragonFruit>(),
			.. HoverTipFactory.FromRelic<LeesWaffle>(),
			.. HoverTipFactory.FromRelic<NutritiousOyster>(),
			.. HoverTipFactory.FromRelic<LoomingFruit>(),
			.. HoverTipFactory.FromRelic<BigMushroom>(),
			.. HoverTipFactory.FromRelic<ChosenCheese>(),
			.. HoverTipFactory.FromRelic<FakeMango>()
		];

		IReadOnlyList<IHoverTip> eightFoodRelicTips =
		[
			.. HoverTipFactory.FromRelic<Strawberry>(),
			.. HoverTipFactory.FromRelic<Mango>(),
			.. HoverTipFactory.FromRelic<Pear>(),
			.. HoverTipFactory.FromRelic<DragonFruit>(),
			.. HoverTipFactory.FromRelic<LeesWaffle>(),
			.. HoverTipFactory.FromRelic<NutritiousOyster>(),
			.. HoverTipFactory.FromRelic<LoomingFruit>(),
			.. HoverTipFactory.FromRelic<ChosenCheese>()
		];

		EventOption oneOption = canAffordOne
			? CreateOption(BuyOne, "TH_YOUMU-FOOD_PURCHASE.pages.INITIAL.options.ONE", randomFoodRelicTips)
			: CreateOption(null, "TH_YOUMU-FOOD_PURCHASE.pages.INITIAL.options.ONE_LOCKED", Array.Empty<IHoverTip>());

		EventOption threeOption = canAffordThree
			? CreateOption(BuyThree, "TH_YOUMU-FOOD_PURCHASE.pages.INITIAL.options.THREE", randomFoodRelicTips)
			: CreateOption(null, "TH_YOUMU-FOOD_PURCHASE.pages.INITIAL.options.THREE_LOCKED", Array.Empty<IHoverTip>());

		EventOption allOption = canAffordAll
			? CreateOption(BuyAll, "TH_YOUMU-FOOD_PURCHASE.pages.INITIAL.options.ALL", eightFoodRelicTips)
			: CreateOption(null, "TH_YOUMU-FOOD_PURCHASE.pages.INITIAL.options.ALL_LOCKED", Array.Empty<IHoverTip>());

		return [oneOption, threeOption, allOption];
	}

	private bool CanAllPlayersAfford(int cost)
	{
		return Owner!.RunState.Players.All(p => p.Gold >= cost);
	}

	private static IReadOnlyList<RelicModel> GetRandomFoodRelicPool()
	{
		return
		[
			ModelDb.Relic<Strawberry>(),
			ModelDb.Relic<Mango>(),
			ModelDb.Relic<Pear>(),
			ModelDb.Relic<DragonFruit>(),
			ModelDb.Relic<LeesWaffle>(),
			ModelDb.Relic<NutritiousOyster>(),
			ModelDb.Relic<LoomingFruit>(),
			ModelDb.Relic<BigMushroom>(),
			ModelDb.Relic<ChosenCheese>(),
			ModelDb.Relic<FakeMango>()
		];
	}

	private static IReadOnlyList<RelicModel> GetEightFixedFoodRelics()
	{
		return
		[
			ModelDb.Relic<Strawberry>(),
			ModelDb.Relic<Mango>(),
			ModelDb.Relic<Pear>(),
			ModelDb.Relic<DragonFruit>(),
			ModelDb.Relic<LeesWaffle>(),
			ModelDb.Relic<NutritiousOyster>(),
			ModelDb.Relic<LoomingFruit>(),
			ModelDb.Relic<ChosenCheese>()
		];
	}

	private async Task BuyOne()
	{
		await PlayerCmd.LoseGold(OneCost, Owner!, GoldLossType.Spent);
		RelicModel relic = Rng.NextItem(GetRandomFoodRelicPool()).ToMutable();
		await RelicCmd.Obtain(relic, Owner!);
		SetEventFinished(PageDescription("ONE"));
	}

	private async Task BuyThree()
	{
		await PlayerCmd.LoseGold(ThreeCost, Owner!, GoldLossType.Spent);
		foreach (RelicModel relic in GetRandomFoodRelicPool().ToList().TakeRandom(3, Rng))
		{
			await RelicCmd.Obtain(relic.ToMutable(), Owner!);
		}
		SetEventFinished(PageDescription("THREE"));
	}

	private async Task BuyAll()
	{
		await PlayerCmd.LoseGold(AllCost, Owner!, GoldLossType.Spent);
		foreach (RelicModel relic in GetEightFixedFoodRelics())
		{
			await RelicCmd.Obtain(relic.ToMutable(), Owner!);
		}
		SetEventFinished(PageDescription("ALL"));
	}
}
