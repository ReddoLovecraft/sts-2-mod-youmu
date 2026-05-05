using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
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
	private const string _bonusDamageKey = "BonusDamage";

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(1, ValueProp.Move),new CardsVar(7), new IntVar(_bonusDamageKey, 0m)];
	
	public BeyondSword() : base(1, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies)
	{
	}

	private void RefreshBonusDamage()
	{
		Player? owner = Owner;
		if (owner == null)
		{
			return;
		}

		int bonus = GetStatusOrCurse(owner).Count();
		base.DynamicVars[_bonusDamageKey].BaseValue = bonus;
	}

	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		RefreshBonusDamage();
		int bonus = base.DynamicVars[_bonusDamageKey].IntValue;
		AttackCommand command = DamageCmd.Attack(base.DynamicVars.Damage.BaseValue + bonus).FromCard(this).TargetingRandomOpponents(base.CombatState).WithHitCount(this.DynamicVars.Cards.IntValue);
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

	public override async Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, CombatState combatState)
	{
		RefreshBonusDamage();
		await Task.CompletedTask;
	}

	public override async Task AfterCardChangedPiles(CardModel card, PileType oldPileType, AbstractModel? source)
	{
		if (Owner != null && card.Owner == Owner)
		{
			RefreshBonusDamage();
		}

		await Task.CompletedTask;
	}
}

}
