using BaseLib.Abstracts;
using BaseLib.Extensions;
using Godot;
using MegaCrit.Sts2.Core.CardSelection;
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



namespace TH_Youmu.Scrpits.Powers
{
    public sealed class CardSlasher : CustomPowerModel
    {
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Counter;
        public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;
        public override string? CustomPackedIconPath => "res://TH_Youmu/ArtWorks/Powers/CSP32.png";
        public override string? CustomBigIconPath => "res://TH_Youmu/ArtWorks/Powers/CSP64.png";
        protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.ForEnergy(this),HoverTipFactory.FromKeyword(CardKeyword.Exhaust)];
        
        public CardSlasher() { }
          public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
        {
            if (player != base.Owner.Player)
            {
                return;
            }
			foreach (CardModel item in await CardSelectCmd.FromHand(prefs: new CardSelectorPrefs(base.SelectionScreenPrompt, 0, Amount), context: choiceContext, player: base.Owner.Player, filter: null, source: this))
            {
                await CardCmd.Exhaust(choiceContext, item);
            }
        }
         public override decimal ModifyMaxEnergy(Player player, decimal amount)
	    {
		if (player != base.Owner.Player)
		{
			return amount;
		}
		    return amount + Amount;
	    }
       
    }

}