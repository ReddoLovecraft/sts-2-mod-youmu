using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;
using Patchoulib.Scrpits.Main;
using TH_Youmu.Scripts.Main;
using TH_Youmu.Scrpits.Powers;

namespace TH_Youmu.Scrpits.Cards
{
[Pool(typeof(YoumuCardPool))]
public class CompassionateSlash : YoumuCardModel
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(33, ValueProp.Move),new CardsVar(4)];
	protected override IEnumerable<IHoverTip> ExtraHoverTips => (new IHoverTip[2]
    {
       HoverTipFactory.FromPower<StiffnessPower>(),
	   HoverTipFactory.FromKeyword(CardKeyword.Exhaust)
    });
	public CompassionateSlash() : base(4, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
	{
	}
	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
	  await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).FromCard(this).TargetingAllOpponents(base.CombatState)
			.WithAttackerAnim("Attack", 0.8f)
			.BeforeDamage(async delegate
			{
				List<Creature> enemies = base.CombatState.Enemies.Where((Creature e) => e.IsAlive).ToList();
				NHyperbeamVfx nHyperbeamVfx = NHyperbeamVfx.Create(base.Owner.Creature, enemies.Last());
				if (nHyperbeamVfx != null)
				{
					NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(nHyperbeamVfx);
					await Cmd.Wait(0.5f);
				}
				foreach (Creature item in enemies)
				{
					NHyperbeamImpactVfx nHyperbeamImpactVfx = NHyperbeamImpactVfx.Create(base.Owner.Creature, item);
					if (nHyperbeamImpactVfx != null)
					{
						NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(nHyperbeamImpactVfx);
					}
				}
			})
			.Execute(choiceContext);
		int finalValue=4;
		List<CardModel> list = GetStatusAndCurse(base.Owner).ToList();
		finalValue-=list.Count;
		foreach (CardModel item in list)
		{
			await CardCmd.Exhaust(choiceContext, item);
		}
		if(finalValue>0&&!Owner.HasPower<StiffnessPower>())
		(await PowerCmd.Apply<StiffnessPower>(base.Owner.Creature, finalValue,Owner.Creature,this)).SetStiffType(StiffType.SpellCard);
	}
	private static IEnumerable<CardModel> GetStatusAndCurse(Player owner)
	{
		return owner.PlayerCombatState.AllCards.Where((CardModel c) => (c.Type == CardType.Status||c.Type == CardType.Curse) && c.Pile.Type != PileType.Exhaust);
	}
	protected override void OnUpgrade()
	{
		DynamicVars.Damage.UpgradeValueBy(7);
	}
}

}
