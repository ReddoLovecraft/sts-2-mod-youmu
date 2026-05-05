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
    public sealed class GhostGardenPower : CustomPowerModel
    {
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Single;
        public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;
        public override string? CustomPackedIconPath => "res://TH_Youmu/ArtWorks/Powers/GGP32.png";
        public override string? CustomBigIconPath => "res://TH_Youmu/ArtWorks/Powers/GGP64.png";
        protected override IEnumerable<IHoverTip> ExtraHoverTips => [Tools.GetStaticKeyword("Derive"),HoverTipFactory.FromKeyword(CardKeyword.Ethereal)];
        
        public GhostGardenPower() { }
       public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
		{
			if (cardPlay.Card.Owner == base.Owner.Player&&cardPlay.Card.Keywords.Contains(CardKeyword.Ethereal))
			{
				List<CardModel> cardModels = await ToolBox.Derive(context,base.Owner.Player,cardPlay.Card.Type,Amount);
                this.Flash();
                if(cardModels.Count>0)
				foreach(CardModel card in cardModels)
				{
					if(!card.Keywords.Contains(CardKeyword.Ethereal))
					{
						CardCmd.ApplyKeyword(card,CardKeyword.Ethereal);
					}
				}
			}
		}
    }

}