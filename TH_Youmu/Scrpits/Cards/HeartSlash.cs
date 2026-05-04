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
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;
using Patchoulib.Scrpits.Main;
using TH_Youmu.Scripts.Main;
using TH_Youmu.Scrpits.Powers;

namespace TH_Youmu.Scrpits.Cards
{
[Pool(typeof(YoumuCardPool))]
public class HeartSlash : YoumuCardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(6, ValueProp.Move),new CardsVar(3)];
	protected override IEnumerable<IHoverTip> ExtraHoverTips => (new IHoverTip[1]
    {
	 	Tools.GetStaticKeyword("Cancel")  
    });
	public override int MaxUpgradeLevel =>3;
	public override CancelType CancelLevel=>CancelType.Final;
	public HeartSlash() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
	{
	}
	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		int drawNum=0;
			CardModel cardModel;
		do
		{
			cardModel = await CardPileCmd.Draw(choiceContext, base.Owner);
			drawNum++;
		}
		while (cardModel != null && cardModel.Type == CardType.Attack && CardPile.GetCards(base.Owner, PileType.Hand).Count() < 10);
		await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue+drawNum*DynamicVars.Cards.IntValue).FromCard(this).Targeting(cardPlay.Target)
			.WithHitFx("vfx/vfx_attack_slash")
			.Execute(choiceContext);
		await ToolBox.Cancel(choiceContext,Owner,this);
	}
	protected override void OnUpgrade()
	{
		this.DynamicVars.Damage.UpgradeValueBy(2);
		this.DynamicVars.Cards.UpgradeValueBy(1);
	}
}

}
