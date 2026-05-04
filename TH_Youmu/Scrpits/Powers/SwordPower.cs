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
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;



namespace TH_Youmu.Scrpits.Powers
{
    public sealed class WhiteSwordPower : CustomPowerModel
    {
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Counter;
        public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;
        public override string? CustomPackedIconPath => "res://TH_Youmu/ArtWorks/Powers/WSP32.png";
        public override string? CustomBigIconPath => "res://TH_Youmu/ArtWorks/Powers/WSP64.png";
       // protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<IgnitePower>(),HoverTipFactory.FromPower<WraithPower>()];
        public WhiteSwordPower() { }
        public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
        {
            if (side == base.Owner.Side)
            {
                Flash();
               await PowerCmd.Decrement(this);
            }
        }
           public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
	    {
		    if (target == base.Owner)
		    {
			    return 1m;
		    }
             if (dealer==null||dealer != base.Owner)
		    {
			    return 1m;
		    }
		    if (!props.IsPoweredAttack())
		    {
			    return 1m;
		    }
		    return 1.25m;
	    }
    }
    public sealed class YellowSwordPower : CustomPowerModel
    {
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Counter;
        public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;
        public override string? CustomPackedIconPath => "res://TH_Youmu/ArtWorks/Powers/YSP32.png";
        public override string? CustomBigIconPath => "res://TH_Youmu/ArtWorks/Powers/YSP64.png";
       // protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<IgnitePower>(),HoverTipFactory.FromPower<WraithPower>()];
        public YellowSwordPower() { }
        public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
        {
            if (side == base.Owner.Side)
            {
                Flash();
               await PowerCmd.Decrement(this);
            }
        }
           public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
	    {
		    if (target == base.Owner)
		    {
			    return 1m;
		    }
             if (dealer==null||dealer != base.Owner)
		    {
			    return 1m;
		    }
		    if (!props.IsPoweredAttack())
		    {
			    return 1m;
		    }
		    return 1.5m;
	    }
    }
     public sealed class RedSwordPower : CustomPowerModel
    {
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Counter;
        public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;
        public override string? CustomPackedIconPath => "res://TH_Youmu/ArtWorks/Powers/RSP32.png";
        public override string? CustomBigIconPath => "res://TH_Youmu/ArtWorks/Powers/RSP64.png";    
       // protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<IgnitePower>(),HoverTipFactory.FromPower<WraithPower>()];
        public RedSwordPower() { }
          public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
        {
            if (side == base.Owner.Side)
            {
                Flash();
               await PowerCmd.Decrement(this);
            }
        }
           public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
	    {
		    if (target == base.Owner)
		    {
			    return 1m;
		    }
             if (dealer==null||dealer != base.Owner)
		    {
			    return 1m;
		    }
		    if (!props.IsPoweredAttack())
		    {
			    return 1m;
		    }
		    return 2m;
	    }

    }

}