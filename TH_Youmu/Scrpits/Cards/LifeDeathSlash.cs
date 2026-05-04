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
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;
using Patchoulib.Scrpits.Main;
using TH_Youmu.Scripts.Main;
using TH_Youmu.Scrpits.Powers;

namespace TH_Youmu.Scrpits.Cards
{
[Pool(typeof(YoumuCardPool))]
public class LifeDeathSlash : YoumuCardModel
{
    public override int MaxUpgradeLevel =>3;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(5, ValueProp.Move),new CardsVar(5)];
	protected override IEnumerable<IHoverTip> ExtraHoverTips => (new IHoverTip[2]
    {
       Tools.GetStaticKeyword("Derive"),
       Tools.GetStaticKeyword("Cancel")
    });
	private decimal _extraDamageFromSlashPlays;

	private decimal ExtraDamageFromSlashPlays
	{
		get
		{
			return _extraDamageFromSlashPlays;
		}
		set
		{
			AssertMutable();
			_extraDamageFromSlashPlays = value;
		}
	}
	public override CancelType CancelLevel=>CancelType.Final;
	public LifeDeathSlash() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
	{
	}
	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).WithHitCount(this.DynamicVars.Cards.IntValue).FromCard(this)
			.Targeting(cardPlay.Target)
			.WithHitVfxNode((Creature t) => NStabVfx.Create(t, facingEnemies: true, VfxColor.Purple))
			.Execute(choiceContext);
		await ToolBox.Derive(choiceContext,Owner,CardType.Attack,1);
		await ToolBox.Cancel(choiceContext,Owner,this);
	}
	 public override async Task TriggerWhenDerive(PlayerChoiceContext choiceContext,Player player,CardType cardType,int amount,bool IsAny=false)
    {
        IEnumerable<LifeDeathSlash> enumerable = base.Owner.PlayerCombatState.AllCards.OfType<LifeDeathSlash>();
		decimal baseValue = base.DynamicVars.Cards.BaseValue;
		foreach (LifeDeathSlash item in enumerable)
		{
			item.BuffFromSlashPlay(baseValue);
		}
    }
	protected override void AfterDowngraded()
	{
		base.AfterDowngraded();
		base.DynamicVars.Damage.BaseValue += ExtraDamageFromSlashPlays;
	}

	private void BuffFromSlashPlay(decimal extraDamage)
	{
		base.DynamicVars.Damage.BaseValue += extraDamage;
		ExtraDamageFromSlashPlays += extraDamage;
	}
	protected override void OnUpgrade()
	{
		DynamicVars.Cards.UpgradeValueBy(2);
		DynamicVars.Damage.UpgradeValueBy(2);
	}
}

}
