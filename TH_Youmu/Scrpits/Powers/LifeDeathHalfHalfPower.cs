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
    public sealed class LifeDeathHalfHalfPower : CustomPowerModel
    {
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Single;
        public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;
        public override string? CustomPackedIconPath => "res://TH_Youmu/ArtWorks/Powers/LDHHP32.png";
        public override string? CustomBigIconPath => "res://TH_Youmu/ArtWorks/Powers/LDHHP64.png";
        
        public LifeDeathHalfHalfPower() { }

		private bool IsAboveHalfHp()
		{
			return base.Owner.CurrentHp * 2 > base.Owner.MaxHp;
		}

		private bool IsBelowHalfHp()
		{
			return base.Owner.CurrentHp * 2 < base.Owner.MaxHp;
		}

		public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
		{
			if (dealer!=null&&props.IsPoweredAttack()&&dealer == base.Owner && IsAboveHalfHp()&&target!=Owner)
			{
				return 2m;
			}
			if (target == base.Owner && IsBelowHalfHp())
			{
				return 0.5m;
			}
			return 1m;
		}

		public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
		{
			if (cardPlay.Card.Owner == null || cardPlay.Card.Owner != base.Owner.Player)
			{
				await Task.CompletedTask;
				return;
			}

			if (IsAboveHalfHp() && cardPlay.Card.Type == CardType.Attack)
			{
				Flash();
				await CreatureCmd.Damage(context, base.Owner, 6, ValueProp.Unblockable | ValueProp.Unpowered, cardSource: cardPlay.Card);
				return;
			}

			if (IsBelowHalfHp() && cardPlay.Card.Type == CardType.Skill)
			{
				Flash();
				await CreatureCmd.Heal(base.Owner, 6);
			}
		}
        
    }

}
