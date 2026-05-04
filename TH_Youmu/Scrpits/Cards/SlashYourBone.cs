using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;
using Patchoulib.Scrpits.Main;
using TH_Youmu.Scripts.Main;
using TH_Youmu.Scrpits.Powers;

namespace TH_Youmu.Scrpits.Cards
{
[Pool(typeof(StatusCardPool))]
public class SlashYourBone : YoumuCardModel
{
   	public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Retain,CardModifier.GuardKeyword];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(0, ValueProp.Move),new CardsVar(2)];
	protected override IEnumerable<IHoverTip> ExtraHoverTips => (new IHoverTip[2]
    {
        HoverTipFactory.FromPower<BufferPower>(),
        HoverTipFactory.FromKeyword(CardKeyword.Exhaust)
    });
	public SlashYourBone() : base(3, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
	{
	}
	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
		int line=0;
		if(cardPlay.Target!=null&&cardPlay.Target.Monster.IntendsToAttack)
		{
			List<Creature> targets = [Owner.Creature];
			foreach (AttackIntent intent in cardPlay.Target.Monster.NextMove.Intents.OfType<AttackIntent>())
            {
                line += intent.GetTotalDamage(targets, cardPlay.Target);
            }
		}
		int lossHp=Owner.Creature.MaxHp-Owner.Creature.CurrentHp;
        int addtionValue=lossHp*this.DynamicVars.Cards.IntValue;
	    AttackCommand attack = DamageCmd.Attack(base.DynamicVars.Damage.BaseValue+addtionValue).FromCard(this)
			.WithAttackerFx(null, "event:/sfx/characters/regent/regent_sovereign_blade");
		if(Owner.Character is YoumuCharacter)
		{
			attack =attack.WithAttackerAnim("Guard", base.Owner.Character.AttackAnimDelay);
		}
		attack = attack.Targeting(cardPlay.Target).WithHitVfxNode((Creature t) => NBigSlashVfx.Create(t))
				.WithHitVfxNode((Creature t) => NBigSlashImpactVfx.Create(t));
		await attack.Execute(choiceContext);
		await PowerCmd.Apply<BufferPower>(Owner.Creature,1,Owner.Creature,this);
		if(attack.Results.Sum((DamageResult r) => r.TotalDamage + r.OverkillDamage)<line)
		{
			await CardCmd.Exhaust(choiceContext,this);
		}
	}
	protected override PileType GetResultPileType()
	{
		PileType resultPileType = base.GetResultPileType();
		if (resultPileType != PileType.Discard)
		{
			return resultPileType;
		}
		return PileType.Hand;
	}
	protected override void OnUpgrade()
	{
		DynamicVars.Cards.UpgradeValueBy(1);
	}
}

}
