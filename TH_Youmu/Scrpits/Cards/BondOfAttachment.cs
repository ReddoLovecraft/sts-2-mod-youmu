using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using TH_Youmu.Scripts.Main;
namespace TH_Youmu.Scrpits.Cards
{
[Pool(typeof(YoumuCardPool))]
public sealed class BondOfAttachment : YoumuCardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new List<DynamicVar> { new CardsVar(1),new DynamicVar("Power",1)};
    protected override IEnumerable<IHoverTip> ExtraHoverTips => (new IHoverTip[2]
        {
          HoverTipFactory.FromPower<VulnerablePower>(),
          HoverTipFactory.FromPower<WeakPower>()
        });

    public BondOfAttachment()
        : base(0, CardType.Skill, CardRarity.Common, TargetType.AllEnemies)
    {
    }
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
       await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);
       await PowerCmd.Apply<WeakPower>(base.CombatState.HittableEnemies, base.DynamicVars["Power"].IntValue,Owner.Creature, this);
       await PowerCmd.Apply<VulnerablePower>(base.CombatState.HittableEnemies, base.DynamicVars["Power"].IntValue,Owner.Creature, this);
       await CardPileCmd.Draw(choiceContext,this.DynamicVars.Cards.IntValue,Owner);
    }
    protected override void OnUpgrade()
    {
        base.DynamicVars["Power"].UpgradeValueBy(1);
    }
}

}
