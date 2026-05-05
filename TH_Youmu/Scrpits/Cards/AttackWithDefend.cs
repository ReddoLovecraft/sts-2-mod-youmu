using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using TH_Youmu.Scripts.Main;
using TH_Youmu.Scrpits.Powers;

namespace TH_Youmu.Scrpits.Cards
{
[Pool(typeof(YoumuCardPool))]
public class AttackWithDefend : YoumuCardModel,ITranscendenceCard
{
	public override bool GainsBlock => true;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(2)];
	public CardModel GetTranscendenceTransformedCard() => ModelDb.Card<DefendInAttack>();
	public AttackWithDefend() : base(1, CardType.Power, CardRarity.Basic, TargetType.Self)
	{
	}
	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);
		await PowerCmd.Apply<AttackWithDefendPower>(Owner.Creature,DynamicVars.Cards.IntValue,Owner.Creature,this);
	}
	protected override void OnUpgrade()
	{
		this.AddKeyword(CardKeyword.Innate);
		this.DynamicVars.Cards.UpgradeValueBy(1);
	}
}

}
