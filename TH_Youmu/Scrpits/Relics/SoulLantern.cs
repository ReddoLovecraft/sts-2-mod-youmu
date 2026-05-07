using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
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
using MegaCrit.Sts2.Core.Saves.Runs;
using Patchouib.Scrpits.Main;
using Patchoulib.Scrpits.Main;
using TH_Youmu.Scripts.Main;
using TH_Youmu.Scrpits.Powers;

namespace TH_Youmu.Relics
{
[Pool(typeof(YoumuRelicPool))]
public class SoulLantern : CustomRelicModel
{
	public override string PackedIconPath => $"res://TH_Youmu/ArtWorks/Relics/{Id.Entry}.png";
    protected override string PackedIconOutlinePath => $"res://TH_Youmu/ArtWorks/Relics/Outlines/{Id.Entry}.png";
    protected override string BigIconPath => $"res://TH_Youmu/ArtWorks/Relics/{Id.Entry}.png";
    public override RelicRarity Rarity => RelicRarity.Uncommon;
	 int amount=0;
	[SavedProperty]
    public  int Counter
    {
        get{return counter;}
        set
        {
            AssertMutable();
			counter=value;
			InvokeDisplayAmountChanged();
        }
    }
	[SavedProperty]
    public  int Amount
    {
        get{return amount;}
        set
        {
            AssertMutable();
			amount=value;
			InvokeDisplayAmountChanged();
        }
    }
	int counter=0;
	public override bool ShowCounter => true;
    public override int DisplayAmount => counter;
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.ForEnergy(this)];
	protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[1]
	{
		new EnergyVar(1)
	};
	 public override async Task AfterRoomEntered(AbstractRoom room)
	{
		if (room is CombatRoom)
		{
		   base.Status = RelicStatus.Active;
		   switch(room.RoomType)
		   {
				case RoomType.Boss:
					amount=5;
					break;
				case RoomType.Elite:
					amount=3;
					break;
				default:
					amount=1;
					break;
		   }
		}
		else
		{
			base.Status = RelicStatus.Normal;
		}
	}
    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != base.Owner)
        {
                return;
        }
		if(Counter<=0)
		{
			return;
		}
		Flash();
        await PlayerCmd.GainEnergy(1, base.Owner);
        await CardPileCmd.Draw(choiceContext, 1, base.Owner);
		Counter--;
		InvokeDisplayAmountChanged();
	}
	public override async Task AfterDeath(PlayerChoiceContext choiceContext, Creature target, bool wasRemovalPrevented, float deathAnimLength)
	{
		if (target.Side != base.Owner.Creature.Side)
		{
			Flash();
            Counter+=Amount;
			InvokeDisplayAmountChanged();
		}
	}

}
}
