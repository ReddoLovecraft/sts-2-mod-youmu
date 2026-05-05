using BaseLib.Abstracts;
using BaseLib.Extensions;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
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
    public sealed class SmartHalfPower : CustomPowerModel
    {
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Counter;
        public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;
        public override string? CustomPackedIconPath => "res://TH_Youmu/ArtWorks/Powers/SHP232.png";
        public override string? CustomBigIconPath => "res://TH_Youmu/ArtWorks/Powers/SHP264.png";
        protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.Static(StaticHoverTip.Block)]; 
        public SmartHalfPower() { }
        public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
        {
            if (player != base.Owner.Player)
            {
                return;
            }
            Flash();
            await PowerCmd.Decrement(this);
        }
        public override async Task AfterAttack(AttackCommand command)
	{
		if (command.Attacker != base.Owner || command.TargetSide == base.Owner.Side || !command.DamageProps.IsPoweredAttack())
		{
			return;
		}
		List<DamageResult> list = command.Results.ToList();
		List<DamageResult> list2 = list.Where((DamageResult r) => r.Receiver.IsPet).ToList();
		foreach (DamageResult petHit in list2)
		{
			list.RemoveAll((DamageResult r) => r.Receiver == petHit.Receiver.PetOwner?.Creature);
		}
		foreach (DamageResult hit in list)
		{
			if (hit.UnblockedDamage > 0)
			{
				await CreatureCmd.GainBlock(Owner,hit.UnblockedDamage,ValueProp.Unpowered,null);
			}
		}
	}
	    
    }

}