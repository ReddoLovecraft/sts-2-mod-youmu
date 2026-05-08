using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;
using TH_Youmu.Relics;
using TH_Youmu.Scrpits.Main;

namespace TH_Youmu.Scrpits.Events;

public sealed class BroSibling : YoumuEventModel
{
	private bool? _danteWins;

	private bool DanteWins
	{
		get
		{
			AssertMutable();
			_danteWins ??= Rng.NextInt(2) == 0;
			return _danteWins.Value;
		}
	}
	public override string? CustomInitialPortraitPath => "res://TH_Youmu/ArtWorks/Events/brosibling.png";
	public override bool IsAllowed(IRunState runState)
	{
		return runState.CurrentActIndex >= 1 && runState.CurrentActIndex <= 2;
	}

	protected override IReadOnlyList<EventOption> GenerateInitialOptions()
	{
		return
		[
			CreateOption(SelectDante, "TH_YOUMU-BRO_SIBLING.pages.INITIAL.options.SELECT_DANTE",
			[
				.. HoverTipFactory.FromRelic(ModelDb.Relic<Chair>())
			]),
			CreateOption(SelectVergil, "TH_YOUMU-BRO_SIBLING.pages.INITIAL.options.SELECT_VERGIL"),
		];
	}

	private async Task SelectDante()
	{
		if (DanteWins)
		{
			await RelicCmd.Obtain(ModelDb.Relic<Chair>().ToMutable(), Owner!);
			SetEventFinished(PageDescription("SELECT_DANTE_SUCCESS"));
			return;
		}
		SetEventFinished(PageDescription("SELECT_DANTE_FAILED"));
	}

	private Task SelectVergil()
	{
		if (!DanteWins)
		{
			IEnumerable<CardModel> upgradableCards = PileType.Deck.GetPile(Owner!).Cards.Where(c => c.IsUpgradable);
			foreach (CardModel card in upgradableCards)
			{
				CardCmd.Upgrade(card);
			}
			SetEventFinished(PageDescription("SELECT_VERGIL_SUCCESS"));
			return Task.CompletedTask;
		}
		SetEventFinished(PageDescription("SELECT_VERGIL_FAILED"));
		return Task.CompletedTask;
	}
}
