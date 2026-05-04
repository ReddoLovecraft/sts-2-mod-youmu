using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

using TH_Youmu.Scripts.Main;
using TH_Youmu.Scrpits.Powers;

namespace TH_Youmu.Scrpits.Relics
{
[Pool(typeof(YoumuRelicPool))]
public class TwoSword : CustomRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Starter;
	public override string PackedIconPath => $"res://TH_Youmu/ArtWorks/Relics/{Id.Entry}.png";
    protected override string PackedIconOutlinePath => $"res://TH_Youmu/ArtWorks/Relics/Outlines/{Id.Entry}.png";
    protected override string BigIconPath => $"res://TH_Youmu/ArtWorks/Relics/{Id.Entry}.png";
	private bool _canTriggerThisTurn = true;
	private CardModel? _trackedCard;
	private Creature? _trackedTarget;
	private int _trackedUnblockedDamage;
	private bool _hasCapturedAoEDamage;
	 protected override IEnumerable<IHoverTip> ExtraHoverTips => (new IHoverTip[3]
        {
		  HoverTipFactory.FromPower<ComboPower>(),
		  HoverTipFactory.FromPower<StiffnessPower>(),
		  HoverTipFactory.FromKeyword(CardKeyword.Exhaust)
        });

	public override async Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, CombatState combatState)
	{
		if (side != base.Owner.Creature.Side)
		{
			return;
		}

		List<CardModel> cardsIn = (from c in PileType.Draw.GetPile(base.Owner).Cards
			orderby c.Rarity, c.Id
			select c).ToList();

		List<CardModel> list = (await CardSelectCmd.FromSimpleGrid(choiceContext, cardsIn, base.Owner, new CardSelectorPrefs(base.SelectionScreenPrompt, 0, 1))).ToList();
		foreach (CardModel item in list)
		{
			await CardCmd.Exhaust(choiceContext, item);
		}

		if (combatState.RoundNumber <= 1)
		{
		(await ComboPower.ApplyAllowZero(Owner.Creature, 0, null, null)).SetCardType(CardType.None);
		}
	}

 	public override async Task AfterSideTurnStart(CombatSide side, CombatState combatState)
	{
		if (side == base.Owner.Creature.Side)
		{
			_canTriggerThisTurn = true;
			_trackedCard = null;
			_trackedTarget = null;
			_trackedUnblockedDamage = 0;
			_hasCapturedAoEDamage = false;
			base.Status = RelicStatus.Active;
		}
	}

	public override Task BeforeCardPlayed(CardPlay cardPlay)
	{
		if (!_canTriggerThisTurn)
		{
			return Task.CompletedTask;
		}
		if (cardPlay.Card.Owner != base.Owner)
		{
			return Task.CompletedTask;
		}
		if (cardPlay.Card.Type != CardType.Attack)
		{
			return Task.CompletedTask;
		}

		_canTriggerThisTurn = false;
		_trackedCard = cardPlay.Card;
		_trackedTarget = cardPlay.Target;
		_trackedUnblockedDamage = 0;
		_hasCapturedAoEDamage = false;
		return Task.CompletedTask;
	}

	public override Task AfterDamageGiven(PlayerChoiceContext choiceContext, Creature? dealer, DamageResult result, ValueProp props, Creature target, CardModel? cardSource)
	{
		if (_trackedCard == null)
		{
			return Task.CompletedTask;
		}
		if (cardSource != _trackedCard)
		{
			return Task.CompletedTask;
		}
		if (dealer != base.Owner.Creature && dealer?.PetOwner != base.Owner)
		{
			return Task.CompletedTask;
		}

		if (_trackedTarget != null)
		{
			if (target == _trackedTarget)
			{
				_trackedUnblockedDamage += result.TotalDamage;
			}
			return Task.CompletedTask;
		}

		if (!_hasCapturedAoEDamage)
		{
			_trackedUnblockedDamage = result.TotalDamage;
			_hasCapturedAoEDamage = true;
		}
		return Task.CompletedTask;
	}

	public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
	{
		if (_trackedCard == null || cardPlay.Card != _trackedCard)
		{
			return;
		}

		int damage = _trackedUnblockedDamage;
		_trackedCard = null;
		_trackedTarget = null;
		_trackedUnblockedDamage = 0;
		_hasCapturedAoEDamage = false;

		if (damage <= 0)
		{
			return;
		}

		CombatState? combatState = base.Owner.Creature.CombatState;
		if (combatState == null)
		{
			return;
		}

		Flash();
		await CreatureCmd.Damage(context, combatState.HittableEnemies, damage, ValueProp.Unpowered, base.Owner.Creature);
		base.Status = RelicStatus.Normal;
	}
		
}
}
