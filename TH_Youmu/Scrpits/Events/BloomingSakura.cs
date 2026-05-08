using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using TH_Youmu.Relics;
using TH_Youmu.Scrpits.Main;

namespace TH_Youmu.Scrpits.Events;

public sealed class BloomingSakura : YoumuEventModel
{
	public override bool IsAllowed(IRunState runState)
	{
		return HasAllYoumu(runState) && runState.CurrentActIndex == 2;
	}
	public override string? CustomInitialPortraitPath => "res://TH_Youmu/ArtWorks/Events/bloomingsakura.png";
	protected override IReadOnlyList<EventOption> GenerateInitialOptions()
	{
		return
		[
			CreateOption(Run, "TH_YOUMU-BLOOMING_SAKURA.pages.INITIAL.options.RUN",
			[
				.. HoverTipFactory.FromRelic(ModelDb.Relic<SakuraPower>())
			]),
			CreateOption(See, "TH_YOUMU-BLOOMING_SAKURA.pages.INITIAL.options.SEE",
			[
				.. HoverTipFactory.FromCardWithCardHoverTips<Regret>()
			])
		];
	}

	private async Task Run()
	{
		int hpLoss = (Owner!.Creature.MaxHp + 1) / 2;
		await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), Owner.Creature, hpLoss, ValueProp.Unblockable | ValueProp.Unpowered, null, null);
		await RelicCmd.Obtain(ModelDb.Relic<SakuraPower>().ToMutable(), Owner);
		SetEventFinished(PageDescription("RUN"));
	}

	private async Task See()
	{
		await CardPileCmd.AddCursesToDeck(Enumerable.Repeat(ModelDb.Card<Regret>(), 1), Owner!);
		SetEventFinished(PageDescription("SEE"));
	}
}
