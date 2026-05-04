using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Patchoulib.Scrpits.Main;
using TH_Youmu.Scripts.Main;
using TH_Youmu.Scrpits.Powers;

namespace TH_Youmu.Scrpits.Relics
{
[Pool(typeof(YoumuRelicPool))]
public class RightSword : CustomRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Starter;
	public override string PackedIconPath => $"res://TH_Youmu/ArtWorks/Relics/{Id.Entry}.png";
    protected override string PackedIconOutlinePath => $"res://TH_Youmu/ArtWorks/Relics/Outlines/{Id.Entry}.png";
    protected override string BigIconPath => $"res://TH_Youmu/ArtWorks/Relics/{Id.Entry}.png";
	    protected override IEnumerable<IHoverTip> ExtraHoverTips => (new IHoverTip[3]
        {
		  HoverTipFactory.FromPower<ComboPower>(),
		  HoverTipFactory.FromPower<StiffnessPower>(),
		  HoverTipFactory.FromKeyword(CardKeyword.Exhaust)
        });
    public override RelicModel? GetUpgradeReplacement() => ModelDb.Relic<TwoSword>();

	public override async Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, CombatState combatState)
	{
		if (side == base.Owner.Creature.Side && combatState.RoundNumber <= 1)
		{
			List<CardModel> cardsIn = (from c in PileType.Draw.GetPile(base.Owner).Cards
			orderby c.Rarity, c.Id
			select c).ToList();
			List<CardModel> list = (await CardSelectCmd.FromSimpleGrid(choiceContext, cardsIn, base.Owner, new CardSelectorPrefs(base.SelectionScreenPrompt, 0, 1))).ToList();
			foreach (CardModel item in list)
			{
				await CardCmd.Exhaust(choiceContext, item);
			}
		(await ComboPower.ApplyAllowZero(Owner.Creature, 0, null, null)).SetCardType(CardType.None);
		}
	}


}
}
