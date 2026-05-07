using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using Patchouib.Scrpits.Main;
using Patchoulib.Scrpits.Main;
using TH_Youmu.Scripts.Main;
using TH_Youmu.Scrpits.Cards;
using TH_Youmu.Scrpits.Powers;

namespace TH_Youmu.Relics
{
[Pool(typeof(YoumuRelicPool))]
public class SakuraPower : CustomRelicModel
{
	public override string PackedIconPath => $"res://TH_Youmu/ArtWorks/Relics/{Id.Entry}.png";
    protected override string PackedIconOutlinePath => $"res://TH_Youmu/ArtWorks/Relics/Outlines/{Id.Entry}.png";
    protected override string BigIconPath => $"res://TH_Youmu/ArtWorks/Relics/{Id.Entry}.png";
    public override RelicRarity Rarity => RelicRarity.Common;
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromCard<TH_Youmu.Scrpits.Cards.Sakura>(),HoverTipFactory.ForEnergy(this)];
	protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[2]
	{
		new EnergyVar(1),
		new CardsVar(1)
	};
   public override async Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, CombatState combatState)
	{
		if (player == base.Owner)
		{
			Flash();
			List<CardModel> list = new List<CardModel>();
			for (int i = 0; i < base.DynamicVars.Cards.IntValue; i++)
			{
				list.Add(combatState.CreateCard<TH_Youmu.Scrpits.Cards.Sakura>(base.Owner));
			}
			CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardsToCombat(list, PileType.Draw, addedByPlayer: true, CardPilePosition.Random));
			await Cmd.Wait(1.6f);
		}
	}
	public override decimal ModifyMaxEnergy(Player player, decimal amount)
	{
		if (player != base.Owner)
		{
			return amount;
		}
		return amount + (decimal)base.DynamicVars.Energy.IntValue;
	}

}
}
