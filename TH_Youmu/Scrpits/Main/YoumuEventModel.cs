using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Runs;
using TH_Youmu.Scripts.Main;

namespace TH_Youmu.Scrpits.Main;

public abstract class YoumuEventModel : CustomEventModel
{
	protected EventOption CreateOption(Func<Task>? onChosen, string optionKey, IEnumerable<IHoverTip>? hoverTips = null)
	{
		LocString title = new LocString(LocTable, optionKey + ".title");
		LocString description = new LocString(LocTable, optionKey + ".description");
		return new EventOption(this, onChosen, title, description, optionKey, hoverTips ?? Enumerable.Empty<IHoverTip>());
	}

	protected static bool HasAnyYoumu(IRunState runState)
	{
		return runState.Players.Any(p => p.Character is YoumuCharacter);
	}

	protected static bool HasAllYoumu(IRunState runState)
	{
		return runState.Players.Count > 0 && runState.Players.All(p => p.Character is YoumuCharacter);
	}
}
