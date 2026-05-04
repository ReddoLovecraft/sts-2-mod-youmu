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
using Patchoulib.Scrpits.Main;
using TH_Youmu.Scripts.Main;



namespace TH_Youmu.Scrpits.Powers
{
    public sealed class StiffnessPower : CustomPowerModel
    {
        public override PowerType Type => PowerType.Debuff;
        public override PowerStackType StackType => PowerStackType.Counter;
        //public override bool IsInstanced => true;
        public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;
        public override string? CustomPackedIconPath => "res://TH_Youmu/ArtWorks/Powers/SP32.png";
        public override string? CustomBigIconPath => "res://TH_Youmu/ArtWorks/Powers/SP64.png";
        protected override IEnumerable<DynamicVar> CanonicalVars => [new StringVar("Type")];
        public StiffType StiffType=StiffType.None;
        public void SetStiffType(StiffType type)
        {
            StiffType=type;
            string message = "";
            switch (StiffType)
            {
                case StiffType.None:
                    message = "Normal Level.";
            if (MegaCrit.Sts2.Core.Localization.LocManager.Instance.Language == "jpn")
            {
                    message = "普通レベル";
            }
            if (MegaCrit.Sts2.Core.Localization.LocManager.Instance.Language == "zhs")
            {
                    message = "普通等级";
            }
                    break;
                case StiffType.Final:
                    message = "Ultimate Skill Level";
                    if (MegaCrit.Sts2.Core.Localization.LocManager.Instance.Language == "jpn")
            {
                    message = "必殺技レベル";
            }
            if (MegaCrit.Sts2.Core.Localization.LocManager.Instance.Language == "zhs")
            {
                    message = "必杀技等级";
            }
                    break;
                case StiffType.SpellCard:
                    message = "SpellCard Level.";
                    if (MegaCrit.Sts2.Core.Localization.LocManager.Instance.Language == "jpn")
            {
                    message = "符卡レベル";
            }
            if (MegaCrit.Sts2.Core.Localization.LocManager.Instance.Language == "zhs")
            {
                    message = "符卡等级";
            }
                    break;
            }
		    ((StringVar)base.DynamicVars["Type"]).StringValue = message;
            InvokeDisplayAmountChanged();
        }
       //protected override IEnumerable<IHoverTip> ExtraHoverTips => [Tools.GetStaticKeyword("Cancel")];
        public StiffnessPower() { }

        public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
        {
            await ApplyLockToAllAttacksInHand();
        }

        public override async Task AfterCardChangedPiles(CardModel card, PileType oldPileType, AbstractModel? source)
        {
            if (card.Owner == base.Owner.Player && oldPileType != PileType.Hand && card.Pile?.Type == PileType.Hand && card.Type == CardType.Attack)
            {
                await CardCmd.Afflict<Scripts.Main.Lock>(card, 1m);
            }
        }

          public override bool ShouldPlay(CardModel card, AutoPlayType autoPlayType)
        {
            if (card.Owner != base.Owner.Player)
            {
                return true;
            }
            return card.Type != CardType.Attack||(card is YoumuCardModel ymc&&CanCancel(ymc));
        }
        public bool CanCancel(YoumuCardModel ymc)
        {
           bool res=false;
           switch (StiffType)
           {
               case StiffType.None:
                   res=ymc.CancelLevel!=CancelType.None;
                   break;
               case StiffType.Final:
                   res=ymc.CancelLevel==CancelType.SpellCard;
                   break;
               case StiffType.SpellCard:
                   res=false;
                   break;
           }
           return res;
        }
         public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
        {
            if (side == base.Owner.Side)
            {
                Flash();
                await PowerCmd.Remove(this);
            }
        }
        public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
		{
			if (cardPlay.Card.Owner == base.Owner.Player&&cardPlay.Card.Type != CardType.Attack)
			{
				await PowerCmd.Decrement(this);
			}
		}

        public override Task AfterRemoved(Creature oldOwner)
        {
            IEnumerable<CardModel> cards = oldOwner.Player?.PlayerCombatState?.AllCards ?? Array.Empty<CardModel>();
            foreach (CardModel item in cards)
            {
                if (item.Affliction is Scripts.Main.Lock)
                {
                    CardCmd.ClearAffliction(item);
                }
            }
            return Task.CompletedTask;
        }

        private async Task ApplyLockToAllAttacksInHand()
        {
            IEnumerable<CardModel> handCards = base.Owner.Player?.PlayerCombatState?.Hand?.Cards ?? Array.Empty<CardModel>();
            foreach (CardModel card in handCards)
            {
                if (card.Type == CardType.Attack)
                {
                    await CardCmd.Afflict<Scripts.Main.Lock>(card, 1m);
                }
            }
        }
    }

}
