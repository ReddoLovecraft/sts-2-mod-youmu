using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;
using Patchoulib.Scrpits.Main;
using TH_Youmu.Scripts.Main;
using TH_Youmu.Scrpits.Powers;

namespace TH_Youmu.Scrpits.Cards
{
[Pool(typeof(YoumuCardPool))]
public class AFlashTwoMonth : YoumuCardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(2)];
	protected override bool ShouldGlowGoldInternal => Success;
	protected override IEnumerable<IHoverTip> ExtraHoverTips => (new IHoverTip[1]
    {
        Tools.GetStaticKeyword("Derive")
    });
	public AFlashTwoMonth() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.None)
	{
	}
	public override async Task TriggerWhenDerive(PlayerChoiceContext choiceContext,Player player,CardType cardType,int amount,bool IsAny=false)
    {
		await CardPileCmd.Draw(choiceContext, this.DynamicVars.Cards.IntValue,Owner);
		if(Success)
		{
			await CardPileCmd.Draw(choiceContext, 1,Owner);
		}
    }
	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		await CardPileCmd.Draw(choiceContext, this.DynamicVars.Cards.IntValue,Owner);
		if(Success)
		{
			await CardPileCmd.Draw(choiceContext, 1,Owner);
		}
	}
	private bool Success
	{
		get
		{
			int num = CombatManager.Instance.History.CardPlaysFinished.Count((CardPlayFinishedEntry e) => e.HappenedThisTurn(base.CombatState) && e.CardPlay.Card.Owner == base.Owner);
			return num < 3;
		}
	}

	protected override void OnUpgrade()
	{
		this.DynamicVars.Cards.UpgradeValueBy(1);
	}
}

}
