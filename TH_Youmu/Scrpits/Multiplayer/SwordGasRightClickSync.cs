using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Models;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Multiplayer.Game;
using MegaCrit.Sts2.Core.Multiplayer.Messages.Game;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Runs;
using TH_Youmu.Scrpits.Powers;

namespace TH_Youmu.Scrpits.Multiplayer;

public static class SwordGasRightClickSync
{
	private const uint SwordGasRightClickChoiceId = 4000000000u;

	private static PlayerChoiceSynchronizer? _lastSynchronizer;

	public static void EnsureSubscribed()
	{
		PlayerChoiceSynchronizer synchronizer = RunManager.Instance.PlayerChoiceSynchronizer;
		if (synchronizer == null)
		{
			return;
		}
		if (ReferenceEquals(_lastSynchronizer, synchronizer))
		{
			return;
		}
		if (_lastSynchronizer != null)
		{
			_lastSynchronizer.PlayerChoiceReceived -= OnPlayerChoiceReceived;
		}
		_lastSynchronizer = synchronizer;
		synchronizer.PlayerChoiceReceived += OnPlayerChoiceReceived;
	}

	public static Task DoLocalAndSync(Player player, CardModel card, int swordGasAmount)
	{
		if (swordGasAmount <= 0)
		{
			return Task.CompletedTask;
		}
		if (card.DynamicVars?.Damage == null)
		{
			return Task.CompletedTask;
		}
		if (RunManager.Instance.NetService.Type.IsMultiplayer())
		{
			uint cardId = NetCombatCardDb.Instance.GetCardId(card);
			PlayerChoiceMessage message = new PlayerChoiceMessage
			{
				choiceId = SwordGasRightClickChoiceId,
				result = PlayerChoiceResult.FromIndexes([(int)cardId, swordGasAmount]).ToNetData()
			};
			RunManager.Instance.NetService.SendMessage(message);
		}
		ApplyImmediate(player, card, swordGasAmount);
		return Task.CompletedTask;
	}

	private static void OnPlayerChoiceReceived(Player player, uint choiceId, NetPlayerChoiceResult result)
	{
		if (choiceId != SwordGasRightClickChoiceId)
		{
			return;
		}
		if (result.type != PlayerChoiceType.Index || result.indexes == null || result.indexes.Count < 2)
		{
			return;
		}
		int cardIndex = result.indexes[0];
		int amount = result.indexes[1];
		if (cardIndex < 0 || amount <= 0)
		{
			return;
		}
		if (!NetCombatCardDb.Instance.TryGetCard((uint)cardIndex, out CardModel? card) || card == null)
		{
			return;
		}
		if (!ReferenceEquals(card.Owner, player))
		{
			return;
		}
		ApplyImmediate(player, card, amount);
	}

	private static void ApplyImmediate(Player player, CardModel card, int swordGasAmount)
	{
		if (swordGasAmount <= 0)
		{
			return;
		}
		if (card.DynamicVars?.Damage == null)
		{
			return;
		}

		player.Creature.GetPower<SwordGasPower>()?.RemoveInternal();
		card.DynamicVars.Damage.UpgradeValueBy(swordGasAmount);
		if (card.Pile != null)
		{
			NCard.FindOnTable(card)?.UpdateVisuals(card.Pile.Type, CardPreviewMode.Normal);
		}
	}
}

[HarmonyPatch(typeof(RunManager))]
[HarmonyPatch("InitializeShared")]
public static class SwordGasRightClickSyncRunManagerPatch
{
	public static void Postfix()
	{
		SwordGasRightClickSync.EnsureSubscribed();
	}
}
