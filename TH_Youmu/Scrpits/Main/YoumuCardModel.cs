using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Patches.Content;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.ValueProps;
using Patchouib.Scrpits.Main;
using TH_Youmu.Scrpits.Cards;
using TH_Youmu.Scrpits.Powers;



namespace TH_Youmu.Scripts.Main
{
	public abstract class YoumuCardModel : CustomCardModel,IRightClickableCardModel
	{
		//public override string PortraitPath => $"res://TH_Youmu/ArtWorks/Cards/{Id.Entry}.png";
		public YoumuCardModel(int baseCost, CardType type, CardRarity rarity, TargetType target, bool showInCardLibrary = true, bool autoAdd = true)
	 : base(baseCost, type, rarity, target, showInCardLibrary)
		{
			if (autoAdd)
			{
				CustomContentDictionary.AddModel(GetType());
			}
		}
		public virtual CancelType CancelLevel=>	Type==CardType.Attack?CancelType.None:CancelType.Skill;

        List<PileType> IRightClickableCardModel.Pile => [PileType.Hand];

        public bool IsCombat => true;

        public override async Task BeforeDamageReceived(PlayerChoiceContext choiceContext, Creature target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
	{
		if (target == base.Owner.Creature && dealer != null && (props.IsPoweredAttack())&&dealer!=base.Owner.Creature&&this.Keywords.Contains(CardModifier.GuardKeyword))
		{
       		CardPile? pile = base.Pile;
		    if (pile != null && pile.Type == PileType.Hand)
		    {
			await ToolBox.TriggerWhenGuard(choiceContext, target, amount, props, dealer, cardSource);
			await TriggerWhenGuard(choiceContext, target, amount, props, dealer, cardSource);
			await CardCmd.AutoPlay(choiceContext, this, dealer);  
		    }
		}
	}
	 public virtual async Task TriggerWhenDerive(PlayerChoiceContext choiceContext,Player player,CardType cardType,int amount,bool IsAny=false)
    {

    }
	public virtual async Task TriggerWhenGuard(PlayerChoiceContext choiceContext, Creature target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
	{
		
	}
	 public virtual async Task TriggerWhenCancel(PlayerChoiceContext choiceContext,Player player,YoumuCardModel currentCard)
    {

    }

    public async Task OnRightClick(PlayerChoiceContext context)
    {
		if(this.Type!=CardType.Attack)
		{
			return;
		}
		int value=0;
		if(Owner.HasPower<SwordGasPower>())
		{
			value=Owner.Creature.GetPowerAmount<SwordGasPower>();
		}
		if(this.DynamicVars.Damage!=null&&value>0)
		{
			await PowerCmd.Remove(Owner.Creature.GetPower<SwordGasPower>());
			this.DynamicVars.Damage.UpgradeValueBy(value);
			CardCmd.Preview(this);
		}
    }

      
    }
  
}
