using BaseLib.Abstracts;
using BaseLib.Extensions;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;
using Patchoulib.Scrpits.Main;



namespace TH_Youmu.Scrpits.Powers
{
    public sealed class ShotBirdEventPower : CustomPowerModel
    {
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Counter;
        public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;
        public override string? CustomPackedIconPath => "res://TH_Youmu/ArtWorks/Powers/SBEP32.png";
        public override string? CustomBigIconPath => "res://TH_Youmu/ArtWorks/Powers/SBEP64.png";
        
        public ShotBirdEventPower() { }
          public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
        {
            if (player != base.Owner.Player)
            {
                return;
            }
            IEnumerable<Creature> enumerable = from c in base.CombatState.GetTeammatesOf(base.Owner)
			where c != null && c.IsAlive && c.IsPlayer
			select c;
		    foreach (Creature item in enumerable)
		    {
			List<CardModel> cardModels = CardFactory.GetDistinctForCombat(item.Player, from c in item.Player.Character.CardPool.GetUnlockedCards(item.Player.UnlockState, item.Player.RunState.CardMultiplayerConstraint)
			where c.Type == CardType.Attack
			select c, Amount, item.Player.RunState.Rng.CombatCardGeneration).ToList();
            if (cardModels.Count()>0)
            foreach (CardModel cardModel in cardModels)
            {
                cardModel.SetToFreeThisTurn();
                await CardPileCmd.AddGeneratedCardToCombat(cardModel, PileType.Hand, addedByPlayer: true);  
            }
		    }
        }
    }

}