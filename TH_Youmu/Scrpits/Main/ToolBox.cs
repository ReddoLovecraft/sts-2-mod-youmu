using System.Threading.Tasks;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;
using TH_Youmu.Scrpits.Powers;

namespace TH_Youmu.Scripts.Main
{
	public static class ToolBox
	{

        public static int GetDebuffTotalCount(Creature target) 
        {
            int result = 0;
            foreach(PowerModel debuff in target.Powers) 
            {
                if(debuff.Type==PowerType.Debuff) 
                {
                    if (debuff.Amount > 0)
                        result += debuff.Amount;
                    else
                        result++;
                }
            }
            return result;

        }
        public static int GetDebuffKind(Creature target)
        {
            int result = 0;
            foreach (PowerModel debuff in target.Powers)
            {
                if (debuff.Type == PowerType.Debuff)
                {
                        result++;
                }
            }
            return result;
        }
             public static LocString L10NStatic(string entry,string targetTable="static_hover_tips")
            {
                return new LocString(targetTable, entry);
            }
            public static LocString GetCustomText(string targetTable,string entry,string postfix)
            {
                string text = StringHelper.Slugify(entry);
                LocString res = L10NStatic(targetTable,text + postfix);
                return res;
            }
            public static bool WasLastCardPlayedSpecificCard(Player player,CardModel currentCard,CardType cardType,bool isAny=false)
	        {
		       CardPlayStartedEntry cardPlayStartedEntry = CombatManager.Instance.History.CardPlaysStarted.LastOrDefault((CardPlayStartedEntry e) => e.CardPlay.Card.Owner == player && e.HappenedThisTurn(player.Creature.CombatState) && e.CardPlay.Card != currentCard);
			   if(isAny)
               {
                    return cardPlayStartedEntry!=null;
               }
               if (cardPlayStartedEntry == null)
			   {
				    return false;
			   }
			   return cardPlayStartedEntry.CardPlay.Card.Type == cardType;
		    }
            public static CardModel GetLastCardPlayed(Player player,CardModel currentCard,CardType cardType,bool isAny=false)
	        {
		       CardPlayStartedEntry cardPlayStartedEntry = CombatManager.Instance.History.CardPlaysStarted.LastOrDefault((CardPlayStartedEntry e) => e.CardPlay.Card.Owner == player && e.HappenedThisTurn(player.Creature.CombatState) && e.CardPlay.Card != currentCard);
			   if(isAny)
               {
                    return cardPlayStartedEntry!=null?cardPlayStartedEntry.CardPlay.Card:null;
               }
               if (cardPlayStartedEntry == null)
			   {
				    return null;
			   }
			   return cardPlayStartedEntry.CardPlay.Card.Type == cardType?cardPlayStartedEntry.CardPlay.Card:null;
		    }
	

		public static async Task TriggerWhenGuard(PlayerChoiceContext choiceContext, Creature target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
		{
			 if(target.HasPower<SeizeReturnPower>())
			 {
                target.GetPower<SeizeReturnPower>().ShowEffect();
				await CardPileCmd.Add(cardSource, PileType.Hand);
			 }
		}
        public static async Task<List<CardModel>> Derive(PlayerChoiceContext choiceContext,Player player,CardType cardType,int amount,bool IsAny=false)
        {
            List<CardModel> result=new List<CardModel>();
             CardSelectorPrefs prefs = new CardSelectorPrefs(GetCustomText("static_hover_tips","derive",".selectionScreenPrompt"), amount);
              List<CardModel> cardsIn = new List<CardModel>();
            if(IsAny)
            {
                cardsIn = PileType.Draw.GetPile(player).Cards.ToList();
            }
            else
            {
                cardsIn = PileType.Draw.GetPile(player).Cards.Where((CardModel c) => c.Type == cardType).ToList();
            }
            List<CardModel> cardModels = (await CardSelectCmd.FromSimpleGrid(choiceContext, cardsIn, player, prefs)).ToList();
            result.AddRange(cardModels);
            if(cardModels.Count>0)
            {
                foreach(CardModel card in cardModels)
                {
                    await CardPileCmd.Add(card, PileType.Hand);
                    if(player.Creature.HasPower<YoumuPower>())
                    {
                        player.Creature.GetPower<YoumuPower>().ShowEffect();
                       int amt=  card.EnergyCost.GetWithModifiers(CostModifiers.Local);
                       if(amt>0)
                       await PlayerCmd.GainEnergy(amt,player);
                    }
                    if(player.Creature.HasPower<HalfHalfHalfPower>())
                    {
                        
                       int cnt=player.Creature.GetPowerAmount<HalfHalfHalfPower>();
                       for(int i=0;i<cnt;i++)
                       {
                            player.Creature.GetPower<HalfHalfHalfPower>().ShowEffect();
                            await CardCmd.AutoPlay(choiceContext, card, null);
                            await CardPileCmd.AddToCombatAndPreview<Clumsy>(player.Creature, PileType.Hand,cnt, addedByPlayer: true);
                       }
                    }
                    if(card is YoumuCardModel ymc)
                    {
                        await ymc.TriggerWhenDerive(choiceContext,player,cardType,amount,IsAny);
                    }
                }
            }
            return result;
        }
       
         public static async Task Cancel(PlayerChoiceContext choiceContext,Player player,YoumuCardModel currentCard)
        {
            CardModel lastPlayedCard = GetLastCardPlayed(player,currentCard,CardType.None,true);
            if(lastPlayedCard==null)
            {
                return;
            }
            if (!CanCancel(currentCard,lastPlayedCard))
            {
                SfxCmd.Play(YoumuInit.ToModSfxPath("TH_Youmu/ArtWorks/SFX/false.wav"));
                return;
            }
            if(player.Creature.HasPower<StiffnessPower>())
            {
                await PowerCmd.Remove(player.Creature.GetPower<StiffnessPower>());
            }
            SfxCmd.Play(YoumuInit.ToModSfxPath("TH_Youmu/ArtWorks/SFX/true.wav"));
            await CardPileCmd.Add(lastPlayedCard, PileType.Hand);
            if(currentCard is YoumuCardModel ymc)
            {
                await ymc.TriggerWhenCancel(choiceContext,player,currentCard);
            }
        }
       
        public static bool CanCancel(YoumuCardModel currentCard,CardModel lastPlayedCard)
        {
           CancelType currentLevel=currentCard.CancelLevel;
           CancelType diffLevel = CancelType.None;
           if(lastPlayedCard is YoumuCardModel ymc)
           {
               diffLevel = ymc.CancelLevel;
           }
           else
           {
               if(lastPlayedCard.Type==CardType.Attack)
               {
                   diffLevel = CancelType.None;
               }
               else
               {
                   diffLevel = CancelType.Skill;
               }
           }
            return (currentLevel>diffLevel)||currentCard.Owner.HasPower<SkyFormPower>();
        }

        public static async Task OpenSword(Player player,CardModel cardSource)
        {
            if(player.HasPower<WhiteSwordPower>())
            {
                await PowerCmd.Remove(player.Creature.GetPower<WhiteSwordPower>());
                await PowerCmd.Apply<YellowSwordPower>(player.Creature,3,player.Creature,cardSource);
                return;
            }
            if(player.HasPower<YellowSwordPower>())
            {
                await PowerCmd.Remove(player.Creature.GetPower<YellowSwordPower>());
                await PowerCmd.Apply<RedSwordPower>(player.Creature,3,player.Creature,cardSource);
                return;
            }
            if(player.HasPower<RedSwordPower>())
            {
                await PowerCmd.Remove(player.Creature.GetPower<RedSwordPower>());
                await PowerCmd.Apply<RedSwordPower>(player.Creature,3,player.Creature,cardSource);
                return;
            }
            await PowerCmd.Apply<WhiteSwordPower>(player.Creature,3,player.Creature,cardSource);
        }


	}
}