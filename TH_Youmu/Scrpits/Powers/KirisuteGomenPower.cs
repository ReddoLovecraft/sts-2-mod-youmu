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
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;
using Patchoulib.Scrpits.Main;



namespace TH_Youmu.Scrpits.Powers
{
    public sealed class KirisuteGomenPower : CustomPowerModel
    {
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Counter;
		public override bool IsInstanced => true;
        public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;
        public override string? CustomPackedIconPath => "res://TH_Youmu/ArtWorks/Powers/KGP32.png";
        public override string? CustomBigIconPath => "res://TH_Youmu/ArtWorks/Powers/KGP64.png";        
        public KirisuteGomenPower() { }
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
		    return base.Amount;
	    }
        public override async Task BeforeDamageReceived(PlayerChoiceContext choiceContext, Creature target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
	    {
		    if (target == base.Owner&&amount>0)
		    {
               this.Flash();
               NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(NGroundFireVfx.Create(base.Owner, VfxColor.Red));
               await PowerCmd.ModifyAmount(this,1,null,null);
		    }
	    }
    }

}