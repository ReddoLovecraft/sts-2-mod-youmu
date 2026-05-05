using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using Patchoulib.Scrpits.Main;
using TH_Youmu.Scripts.Main;
using TH_Youmu.Scrpits.Powers;

namespace TH_Youmu.Scrpits.Cards
{
[Pool(typeof(YoumuCardPool))]
public class SakuraSword : YoumuCardModel
{
   
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(7, ValueProp.Move)];
	protected override IEnumerable<IHoverTip> ExtraHoverTips => (new IHoverTip[2]
    {
		HoverTipFactory.FromCard<SakuraFlash>(),
        HoverTipFactory.FromCard<Sakura>(base.IsUpgraded)
    });
	public SakuraSword() : base(1, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies)
	{
	}
	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		List<CardModel> list = PileType.Hand.GetPile(base.Owner).Cards.ToList();
		int cardCount = list.Count;
		foreach (CardModel item in list)
		{
			if(item is SakuraFlash)
			{
				continue;
			}
			await CardCmd.Discard(choiceContext, item);
		}
		foreach (Creature enemy in base.CombatState.HittableEnemies.ToList())
		{
			VfxCmd.PlayOnCreature(enemy, "vfx/vfx_attack_slash");
			await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).FromCard(this).WithHitFx("vfx/vfx_flying_slash").WithHitCount(cardCount)
			.Targeting(enemy).Execute(choiceContext);
		}
		for (int i = 0; i < cardCount; i++)
		{
			CardModel card = base.CombatState.CreateCard<Sakura>(base.Owner);
			if (base.IsUpgraded)
			{
				CardCmd.Upgrade(card);
			}
			CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Draw, addedByPlayer: true));
		}
	}
	protected override void OnUpgrade()
	{
		DynamicVars.Damage.UpgradeValueBy(4); 
	}
}

}
