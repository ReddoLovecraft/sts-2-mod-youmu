using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves.Runs;
using Patchouib.Scrpits.Main;
using Patchoulib.Scrpits.Main;
using TH_Youmu.Scripts.Main;
using TH_Youmu.Scrpits.Powers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TH_Youmu.Relics
{
[Pool(typeof(YoumuRelicPool))]
public class Lightsaber : CustomRelicModel,IRightCilckable
{
	private const decimal _hpLossMultiplier = 1.5m;
	private readonly Dictionary<Creature, decimal> _pendingHpLossByTarget = new();

	public override string PackedIconPath => $"res://TH_Youmu/ArtWorks/Relics/{Id.Entry}.png";
    protected override string PackedIconOutlinePath => $"res://TH_Youmu/ArtWorks/Relics/Outlines/{Id.Entry}.png";
    protected override string BigIconPath => $"res://TH_Youmu/ArtWorks/Relics/{Id.Entry}.png";
    public override RelicRarity Rarity => RelicRarity.Uncommon;

	[SavedProperty]
	public bool IsEnabled
	{
		get => _isEnabled;
		set
		{
			AssertMutable();
			_isEnabled = value;
			base.Status = _isEnabled ? RelicStatus.Normal : RelicStatus.Disabled;
		}
	}

	private bool _isEnabled = true;

	public Task OnRightClick(PlayerChoiceContext context)
	{
		IsEnabled = !IsEnabled;
		Flash();
		return Task.CompletedTask;
	}

	public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
	{
		if (!IsEnabled)
		{
			return 1m;
		}
		if (!props.IsPoweredAttack())
		{
			return 1m;
		}
		if (dealer != base.Owner.Creature)
		{
			return 1m;
		}
		if (target == null || target == base.Owner.Creature)
		{
			return 1m;
		}
		decimal baseDamage = amount;
		DamageVar damageVar = cardSource?.DynamicVars?.Damage;
		if (damageVar != null)
		{
			baseDamage = damageVar.BaseValue;
		}
		_pendingHpLossByTarget[target] = baseDamage;
		return 0m;
	}

	public override async Task BeforeDamageReceived(PlayerChoiceContext choiceContext, Creature target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
	{
		if (!IsEnabled)
		{
			return;
		}
		if (!props.IsPoweredAttack())
		{
			return;
		}
		if (dealer != base.Owner.Creature)
		{
			return;
		}
		if (!_pendingHpLossByTarget.TryGetValue(target, out decimal baseDamage))
		{
			return;
		}
		_pendingHpLossByTarget.Remove(target);
		if (baseDamage <= 0m)
		{
			return;
		}
		Flash();
		await CreatureCmd.Damage(choiceContext, target, baseDamage * _hpLossMultiplier, ValueProp.Unblockable | ValueProp.Unpowered, base.Owner.Creature, null);
	}
}
}
