using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Gold;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;
using TH_Youmu.Scrpits.Main;

namespace TH_Youmu.Scrpits.Events;

public sealed class MeetReisen : YoumuEventModel
{
	private const int HealCost = 100;
	private const int GrowthCost = 150;
	private const int RandomCost = 200;
	public override string? CustomInitialPortraitPath => "res://TH_Youmu/ArtWorks/Events/meetreisen.png";

	public override bool IsAllowed(IRunState runState)
	{
		return HasAnyYoumu(runState) && runState.CurrentActIndex <= 1 && runState.Players.All(p => p.Gold >= HealCost);
	}

	protected override IReadOnlyList<EventOption> GenerateInitialOptions()
	{
		bool canHeal = CanAllPlayersAfford(HealCost);
		bool canGrowth = CanAllPlayersAfford(GrowthCost);
		bool canRandom = CanAllPlayersAfford(RandomCost);

		EventOption healOption = canHeal
			? CreateOption(Heal, "TH_YOUMU-MEET_REISEN.pages.INITIAL.options.HEAL")
			: CreateOption(null, "TH_YOUMU-MEET_REISEN.pages.INITIAL.options.HEAL_LOCKED");

		EventOption growthOption = canGrowth
			? CreateOption(Growth, "TH_YOUMU-MEET_REISEN.pages.INITIAL.options.GROWTH")
			: CreateOption(null, "TH_YOUMU-MEET_REISEN.pages.INITIAL.options.GROWTH_LOCKED");

		EventOption randomOption = canRandom
			? CreateOption(Random, "TH_YOUMU-MEET_REISEN.pages.INITIAL.options.RANDOM")
			: CreateOption(null, "TH_YOUMU-MEET_REISEN.pages.INITIAL.options.RANDOM_LOCKED");

		return [healOption, growthOption, randomOption];
	}

	private bool CanAllPlayersAfford(int cost)
	{
		return Owner!.RunState.Players.All(p => p.Gold >= cost);
	}

	private async Task Heal()
	{
		await PlayerCmd.LoseGold(HealCost, Owner!, GoldLossType.Spent);
		int missingHp = Owner!.Creature.MaxHp - Owner.Creature.CurrentHp;
		if (missingHp > 0)
		{
			await CreatureCmd.Heal(Owner.Creature, missingHp);
		}
		SetEventFinished(PageDescription("HEAL"));
	}

	private async Task Growth()
	{
		await PlayerCmd.LoseGold(GrowthCost, Owner!, GoldLossType.Spent);
		await CreatureCmd.GainMaxHp(Owner!.Creature, 15);
		SetEventFinished(PageDescription("GROWTH"));
	}

	private async Task Random()
	{
		await PlayerCmd.LoseGold(RandomCost, Owner!, GoldLossType.Spent);
		foreach (var potion in PotionFactory.CreateRandomPotionsOutOfCombat(Owner!, 3, Owner!.RunState.Rng.CombatPotionGeneration))
		{
			await PotionCmd.TryToProcure(potion.ToMutable(), Owner!);
		}
		SetEventFinished(PageDescription("RANDOM"));
	}
}
