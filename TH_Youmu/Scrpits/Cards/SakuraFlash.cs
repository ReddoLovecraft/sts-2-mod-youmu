using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;
using Patchoulib.Scrpits.Main;
using TH_Youmu.Scripts.Main;
using TH_Youmu.Scrpits.Powers;

namespace TH_Youmu.Scrpits.Cards
{
[Pool(typeof(YoumuCardPool))]
public class SakuraFlash : YoumuCardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(3)];
	protected override IEnumerable<IHoverTip> ExtraHoverTips => (new IHoverTip[5]
    {
	   HoverTipFactory.FromKeyword(CardKeyword.Exhaust),
	   HoverTipFactory.FromCard<Sakura>(),
	   HoverTipFactory.FromPower<IntangiblePower>(),
	   Tools.GetStaticKeyword("Derive"),
	   Tools.GetStaticKeyword("Cancel")
    });
	public override CancelType CancelLevel=>CancelType.SpellCard;
	public SakuraFlash() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
	{
	}
	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);
		await CardPileCmd.Draw(choiceContext,this.DynamicVars.Cards.IntValue,Owner);
		List<CardModel> list =GetSakura(Owner).ToList();
		foreach (CardModel item in list)
		{
			await CardCmd.Exhaust(choiceContext, item);
		}
		int amount=list.Count;
		await PowerCmd.Apply<IntangiblePower>(Owner.Creature,amount,Owner.Creature,this);
		await ToolBox.Cancel(choiceContext,Owner,this);
	}
	 public override async Task TriggerWhenDerive(PlayerChoiceContext choiceContext,Player player,CardType cardType,int amount,bool IsAny=false)
    {
		for (int i = 0; i < this.DynamicVars.Cards.IntValue; i++)
		{
			CardModel card = base.CombatState.CreateCard<Sakura>(base.Owner);
			CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Draw, addedByPlayer: true));
		}
    }
	private static IEnumerable<CardModel> GetSakura(Player owner)
	{
		return owner.PlayerCombatState.AllCards.Where((CardModel c) => c is Sakura && (c.Pile.Type==PileType.Hand||c.Pile.Type==PileType.Draw));
	}
	protected override void OnUpgrade()
	{
		DynamicVars.Cards.UpgradeValueBy(1);
	}
}

}
