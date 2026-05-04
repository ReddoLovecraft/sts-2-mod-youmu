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



namespace TH_Youmu.Scrpits.Powers
{
    public sealed class AthoughtCountlessKalpasPower : CustomPowerModel
    {
        private readonly Dictionary<CardModel, decimal> _originalCostsThisCombat = new();

        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Single;
        public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;
        public override string? CustomPackedIconPath => "res://TH_Youmu/ArtWorks/Powers/ACKP32.png";
        public override string? CustomBigIconPath => "res://TH_Youmu/ArtWorks/Powers/ACKP64.png";
        protected override IEnumerable<IHoverTip> ExtraHoverTips => (new IHoverTip[1]
    {
		HoverTipFactory.FromPower<WasteAwayPower>()
    });
        
        public AthoughtCountlessKalpasPower() { }

        public override bool TryModifyEnergyCostInCombat(CardModel card, decimal originalCost, out decimal modifiedCost)
        {
            if (base.Owner.Player == null || card.Owner != base.Owner.Player)
            {
                modifiedCost = originalCost;
                return false;
            }

            _originalCostsThisCombat[card] = originalCost;

            if (originalCost <= 0m)
            {
                modifiedCost = originalCost;
                return false;
            }

            modifiedCost = 0m;
            return true;
        }

        public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
        {
            if (base.Owner.Player == null || cardPlay.Card.Owner != base.Owner.Player)
            {
                await Task.CompletedTask;
                return;
            }

            if (!_originalCostsThisCombat.TryGetValue(cardPlay.Card, out decimal originalCost) || originalCost <= 0m)
            {
                await Task.CompletedTask;
                return;
            }

            Flash();
            await PowerCmd.Apply<WasteAwayPower>(base.Owner, originalCost, base.Owner, cardPlay.Card);
        }
        
	    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
        {
            if (side != base.Owner.Side)
            {
                return;
            }
            await PowerCmd.Remove(this);
        }
    }

}
