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
		return HasAnyYoumu(runState) && runState.CurrentActIndex <= 1;
	}

	protected override IReadOnlyList<EventOption> GenerateInitialOptions()
	{
		return
		[
			CreateOption(Heal, "TH_YOUMU-MEET_REISEN.pages.INITIAL.options.HEAL"),
			CreateOption(Growth, "TH_YOUMU-MEET_REISEN.pages.INITIAL.options.GROWTH"),
			CreateOption(Random, "TH_YOUMU-MEET_REISEN.pages.INITIAL.options.RANDOM")
		];
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
