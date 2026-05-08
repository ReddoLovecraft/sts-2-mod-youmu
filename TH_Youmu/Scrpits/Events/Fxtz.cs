using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;
using TH_Youmu.Scrpits.Cards;
using TH_Youmu.Scrpits.Main;

namespace TH_Youmu.Scrpits.Events;

public sealed class Fxtz : YoumuEventModel
{
	public override bool IsAllowed(IRunState runState)
	{
		return HasAnyYoumu(runState);
	}
	public override string? CustomInitialPortraitPath => "res://TH_Youmu/ArtWorks/Events/fxtz.png";
	protected override IReadOnlyList<EventOption> GenerateInitialOptions()
	{
		return
		[
			CreateOption(Power, "TH_YOUMU-FXTZ.pages.INITIAL.options.POWER",
			[
				.. HoverTipFactory.FromCardWithCardHoverTips<Fastest>()
			]),
			CreateOption(Skill, "TH_YOUMU-FXTZ.pages.INITIAL.options.SKILL",
			[
				.. HoverTipFactory.FromCardWithCardHoverTips<ManbaOut>()
			]),
			CreateOption(Attack, "TH_YOUMU-FXTZ.pages.INITIAL.options.ATTACK",
			[
				.. HoverTipFactory.FromCardWithCardHoverTips<LightingWhirlwindSlash>()
			])
		];
	}

	private async Task Power()
	{
		CardModel card = Owner!.RunState.CreateCard<Fastest>(Owner!);
		CardCmd.PreviewCardPileAdd(await CardPileCmd.Add(card, PileType.Deck));
		SetEventFinished(PageDescription("POWER"));
	}

	private async Task Skill()
	{
		CardModel card = Owner!.RunState.CreateCard<ManbaOut>(Owner!);
		CardCmd.PreviewCardPileAdd(await CardPileCmd.Add(card, PileType.Deck));
		SetEventFinished(PageDescription("SKILL"));
	}

	private async Task Attack()
	{
		CardModel card = Owner!.RunState.CreateCard<LightingWhirlwindSlash>(Owner!);
		CardCmd.PreviewCardPileAdd(await CardPileCmd.Add(card, PileType.Deck));
		SetEventFinished(PageDescription("ATTACK"));
	}
}
