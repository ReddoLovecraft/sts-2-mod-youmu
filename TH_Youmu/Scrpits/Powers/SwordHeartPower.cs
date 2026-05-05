using BaseLib.Abstracts;
using BaseLib.Extensions;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;



namespace TH_Youmu.Scrpits.Powers
{
    public sealed class SwordHeartPower : CustomPowerModel
    {
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Counter;
        public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;
        public override string? CustomPackedIconPath => "res://TH_Youmu/ArtWorks/Powers/SHP32.png";
        public override string? CustomBigIconPath => "res://TH_Youmu/ArtWorks/Powers/SHP64.png";
        protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.ForEnergy(this),HoverTipFactory.FromPower<StrengthPower>(),HoverTipFactory.FromPower<DexterityPower>()];
        public SwordHeartPower() { }
           public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
        {
            if (player != base.Owner.Player)
            {
                return;
            }
            foreach(Creature mos in Owner.CombatState.HittableEnemies)
            {
                this.Flash();
                bool attackFlag=mos.Monster.NextMove.Intents.Any(delegate(AbstractIntent intent)
		        {
			    IntentType intentType = intent.IntentType;
			    return intentType == IntentType.Attack || intentType == IntentType.DeathBlow ? true : false;
		        });
		        if(attackFlag)
		        await PowerCmd.Apply<DexterityPower>(Owner,Amount,Owner,null);
		        bool DefendFlag=mos.Monster.NextMove.Intents.Any(delegate(AbstractIntent intent)
		        {
			    IntentType intentType = intent.IntentType;
			    return intentType == IntentType.Defend ? true : false;
		        });
		        if(DefendFlag)
		        await PowerCmd.Apply<StrengthPower>(Owner,Amount,Owner,null);
		        bool OtherFlag=mos.Monster.NextMove.Intents.Any(delegate(AbstractIntent intent)
		        {
			    IntentType intentType = intent.IntentType;
			    return intentType != IntentType.Attack && intentType != IntentType.DeathBlow && intentType != IntentType.Defend ? true : false;
		        });
		        if(OtherFlag)
		        {
			    await PlayerCmd.GainEnergy(Amount,Owner.Player);
			    await CardPileCmd.Draw(choiceContext,Amount,Owner.Player);
		        }
            }
			
        }
    }

}