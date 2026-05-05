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
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;
using Patchoulib.Scrpits.Main;
using TH_Youmu.Scripts.Main;
using TH_Youmu.Scrpits.Powers;

namespace TH_Youmu.Scrpits.Cards
{
[Pool(typeof(YoumuCardPool))]
public class ReflectSlash : YoumuCardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(12, ValueProp.Move)];
	public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Retain,CardModifier.GuardKeyword];
	public override int MaxUpgradeLevel =>3;
	public override bool GainsBlock => true;
	protected override bool ShouldGlowGoldInternal => base.CombatState?.HittableEnemies.Any((Creature e) => e.Monster.IntendsToAttack) ?? false;
	public ReflectSlash() : base(3, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy)
	{
	}
	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		await CreatureCmd.GainBlock(base.Owner.Creature, base.DynamicVars.Block, cardPlay);
        if(cardPlay.Target!=null&&cardPlay.Target.Monster.IntendsToAttack)
		{
			int totalIntent=0;
			List<Creature> targets = [Owner.Creature];
			foreach (AttackIntent intent in cardPlay.Target.Monster.NextMove.Intents.OfType<AttackIntent>())
            {
                totalIntent += intent.GetTotalDamage(targets, cardPlay.Target);
            }
			if(Owner.Character is YoumuCharacter)
			{
				await DamageCmd.Attack(totalIntent).FromCard(this).Targeting(cardPlay.Target)
				.WithHitFx("vfx/vfx_attack_slash", null, "slash_attack.mp3").WithAttackerAnim("Guard", base.Owner.Character.AttackAnimDelay).Execute(choiceContext);;
			}
			else
			{
				await DamageCmd.Attack(totalIntent).FromCard(this).Targeting(cardPlay.Target)
				.WithHitFx("vfx/vfx_attack_slash", null, "slash_attack.mp3").Execute(choiceContext);;
			}
		}
	}
	protected override void OnUpgrade()
	{
		this.DynamicVars.Block.UpgradeValueBy(4);
	}
}

}
