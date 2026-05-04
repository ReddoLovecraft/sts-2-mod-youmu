using BaseLib.Patches.Content;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace TH_Youmu.Scrpits.Cards
{
    public class CardModifier 
    {
        [CustomEnum("GUARD")]
        [KeywordProperties(AutoKeywordPosition.After)]
        public static CardKeyword GuardKeyword;
    }
}