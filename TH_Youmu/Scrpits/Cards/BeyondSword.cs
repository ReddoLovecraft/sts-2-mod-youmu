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
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Events;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;
using Patchoulib.Scrpits.Main;
using TH_Youmu.Scripts.Main;
using TH_Youmu.Scrpits.Powers;

namespace TH_Youmu.Scrpits.Cards
{
[Pool(typeof(YoumuCardPool))]
public class BeyondSword : YoumuCardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(1, ValueProp.Move),new CardsVar(7)];
	
	public BeyondSword() : base(1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
	{
	}
	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		int dmgAddtion=GetStatusOrCurse(base.Owner).Count();
		AttackCommand command = DamageCmd.Attack(base.DynamicVars.Damage.BaseValue+dmgAddtion).FromCard(this).TargetingRandomOpponents(base.CombatState).WithHitCount(this.DynamicVars.Cards.IntValue);
		command.WithHitFx("vfx/hellraiser_attack_vfx", command.HitSfx, command.TmpHitSfx).
		WithAttackerAnim("Attack", command.Attacker.Player.Character.CastAnimDelay).SpawningHitVfxOnEachCreature().WithHitVfxSpawnedAtBase();
		await command.Execute(choiceContext);
	}
	private static IEnumerable<CardModel> GetStatusOrCurse(Player owner)
	{
		return owner.PlayerCombatState.AllCards.Where((CardModel c) => c.Type==CardType.Status||c.Type==CardType.Curse);
	}
	protected override void OnUpgrade()
	{
		DynamicVars.Cards.UpgradeValueBy(2);
	}
}

}
