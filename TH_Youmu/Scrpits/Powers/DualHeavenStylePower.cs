using BaseLib.Abstracts;
using BaseLib.Extensions;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;



namespace TH_Youmu.Scrpits.Powers
{
    public sealed class DualHeavenStylePower : CustomPowerModel
    {
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Single;
        public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;
        public override string? CustomPackedIconPath => "res://TH_Youmu/ArtWorks/Powers/DHSP32.png";
        public override string? CustomBigIconPath => "res://TH_Youmu/ArtWorks/Powers/DHSP64.png";
        public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
	    {
		if (dealer != base.Owner)
		{
			return 1m;
		}
		if (!props.IsPoweredAttack())
		{
			return 1m;
        }
       	if (cardSource == null||cardSource.Type!=CardType.Attack)
		{
			return 1m;
        }
        if(Owner.Player.GetRelic<BookOfFiveRings>()!=null)
        {
			return 1m;
	    }
        return 0.75m;
        }
        public DualHeavenStylePower() { }
        public override int ModifyCardPlayCount(CardModel card, Creature? target, int playCount)
	    {
		    if (card.Owner.Creature != base.Owner||card.Type!=CardType.Attack)
		    {
			    return playCount;
		    }
		    return playCount+1;
	    }
	    
    }

}