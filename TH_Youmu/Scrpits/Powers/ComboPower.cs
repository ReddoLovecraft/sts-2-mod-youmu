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
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;
using System.Reflection;



namespace TH_Youmu.Scrpits.Powers
{
    public sealed class ComboPower : CustomPowerModel
    {
        private static readonly MethodInfo? AmountSetter = typeof(PowerModel)
            .GetProperty("Amount", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            ?.GetSetMethod(true);

        public static async Task<ComboPower> ApplyAllowZero(Creature target, int amount, Creature? applier, CardModel? cardSource)
        {
            if (target.HasPower<ComboPower>())
            {
                ComboPower existing = target.GetPower<ComboPower>();
                if (amount <= 0)
                {
                    existing.SetAmountUnsafe(0);
                    existing.SetCardType(CardType.None);
                }
                return existing;
            }

            if (amount <= 0)
            {
                ComboPower created = await PowerCmd.Apply<ComboPower>(target, 1, applier, cardSource);
                created.SetAmountUnsafe(0);
                created.SetCardType(CardType.None);
                return created;
            }

            return await PowerCmd.Apply<ComboPower>(target, amount, applier, cardSource);
        }

        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Counter;
        public override bool AllowNegative => true;
        public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;
        public override string? CustomPackedIconPath => "res://TH_Youmu/ArtWorks/Powers/CP32.png";
        public override string? CustomBigIconPath => "res://TH_Youmu/ArtWorks/Powers/CP64.png";
         protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<StiffnessPower>()];

        public CardType lastCardType = CardType.None;
        protected override IEnumerable<DynamicVar> CanonicalVars => [new StringVar("Type")];
        public void SetCardType(CardType type)
        {
            lastCardType=type;
            string message = "";
            switch (lastCardType)
            {
                case CardType.Attack:
                    message = "Attack";
                    if (MegaCrit.Sts2.Core.Localization.LocManager.Instance.Language == "jpn")
            {
                    message = "攻撃";
            }
            if (MegaCrit.Sts2.Core.Localization.LocManager.Instance.Language == "zhs")
            {
                    message = "攻击";
            }
                    break;
                case CardType.Skill:
                    message = "Skill";
                    if (MegaCrit.Sts2.Core.Localization.LocManager.Instance.Language == "jpn")
            {
                    message = "スキル";
            }
            if (MegaCrit.Sts2.Core.Localization.LocManager.Instance.Language == "zhs")
            {
                    message = "技能";
            }
                    break;
                case CardType.Power:
                    message = "Power";
                    if (MegaCrit.Sts2.Core.Localization.LocManager.Instance.Language == "jpn")
            {
                    message = "能力";
            }
            if (MegaCrit.Sts2.Core.Localization.LocManager.Instance.Language == "zhs")
            {
                    message = "能力";
            }
                    break;
                case CardType.None:
                    message = "None";
                 if (MegaCrit.Sts2.Core.Localization.LocManager.Instance.Language == "jpn")
            {
                    message = "無";
            }
            if (MegaCrit.Sts2.Core.Localization.LocManager.Instance.Language == "zhs")
            {
                    message = "无";
            }
                    break;
                case CardType.Status:
                    message = "Status";
            if (MegaCrit.Sts2.Core.Localization.LocManager.Instance.Language == "jpn")
            {
                    message = "状態";
            }
            if (MegaCrit.Sts2.Core.Localization.LocManager.Instance.Language == "zhs")
            {
                    message = "状态";
            }
                    break;
                case CardType.Curse:
                    message = "Curse";
            if (MegaCrit.Sts2.Core.Localization.LocManager.Instance.Language == "jpn")
            {
                    message = "呪い";
            }
            if (MegaCrit.Sts2.Core.Localization.LocManager.Instance.Language == "zhs")
            {
                    message = "诅咒";
            }
                    break;
                default:
                    message = "Unknown";
                     if (MegaCrit.Sts2.Core.Localization.LocManager.Instance.Language == "jpn")
            {
                    message = "未知";
            }
            if (MegaCrit.Sts2.Core.Localization.LocManager.Instance.Language == "zhs")
            {
                    message = "未知";
            }
                    break;
            }
		    ((StringVar)base.DynamicVars["Type"]).StringValue = message;
            InvokeDisplayAmountChanged();
        }

       
        public ComboPower() { }
        public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
        {
            if (side == base.Owner.Side)
            {
               Flash();
               await ResetCounter();
            }
        }
        public override async Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, CombatState combatState)
		{
			if (side != CombatSide.Player)
			{
				return;
			}
			await ResetCounter();
		}

        public void SetAmountUnsafe(int amount)
        {
            if (AmountSetter != null)
            {
                AmountSetter.Invoke(this, [amount]);
                InvokeDisplayAmountChanged();
            }
        }

        public Task ResetCounter()
        {
            if (AmountSetter != null)
            {
                AmountSetter.Invoke(this, [0]);
                SetCardType(CardType.None);
                return Task.CompletedTask;
            }

            return ResetCounterFallback();
        }

        private async Task ResetCounterFallback()
        {
            await PowerCmd.ModifyAmount(this, -this.Amount, null, null);
            SetCardType(CardType.None);
        }
            public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
		{
			if (cardPlay.Card.Owner == base.Owner.Player)
			{
                this.Flash();
                if(cardPlay.Card.Type!=lastCardType)
                {
                    if(Owner.HasPower<BizarreSixRealmsPower>())
                    {
                        BizarreSixRealmsPower power=Owner.GetPower<BizarreSixRealmsPower>();
                        int amt=Owner.GetPowerAmount<BizarreSixRealmsPower>();
                        if(lastCardType==CardType.Attack)
                        {
                           power.ShowEffect();
                           await PowerCmd.Apply<StrengthPower>(Owner,amt,Owner,null);
                        }
                        else if(lastCardType==CardType.Skill)
                        {
                            power.ShowEffect();
                            await PowerCmd.Apply<DexterityPower>(Owner,amt,Owner,null);
                        }
                        else if(lastCardType==CardType.Power)
                        {
                            power.ShowEffect();
                            await PlayerCmd.GainEnergy(amt,Owner.Player);
                            await CardPileCmd.Draw(context,amt,Owner.Player);
                        }
                    }
                    if(!Owner.HasPower<StiffnessPower>()&&this.Amount>=2)
                    (await PowerCmd.Apply<StiffnessPower>(Owner,this.Amount,null,null)).SetStiffType(Scripts.Main.StiffType.None);
                    await ResetCounter();
                }
                await PowerCmd.ModifyAmount(this,1,null,null);
                this.SetCardType(cardPlay.Card.Type);
			}
		}
        public override bool TryModifyEnergyCostInCombat(CardModel card, decimal originalCost, out decimal modifiedCost)
	{
		if (card.Owner != base.Owner.Player||card.Type!=lastCardType)
		{
			modifiedCost = originalCost;
			return false;
		}
		modifiedCost = originalCost - (decimal)base.Amount;
        modifiedCost = modifiedCost>0?modifiedCost:0;
		return true;
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
            decimal addtion=0;
            if(Owner.HasPower<StillWaterPower>())
            {
                addtion=+Owner.GetPowerAmount<StillWaterPower>()/10m;
            }
            decimal finalValue=(1+(addtion-0.1m)*this.Amount);
		    return (finalValue>0?finalValue:0);
	    }
    }

}
