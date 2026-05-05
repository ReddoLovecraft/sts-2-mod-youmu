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
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;



namespace TH_Youmu.Scrpits.Powers
{
    public sealed class WindWoodFireMountainPower : CustomPowerModel
    {
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Counter;
        public override bool IsInstanced => true;
        public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;
        public override string? CustomPackedIconPath => "res://TH_Youmu/ArtWorks/Powers/WWFMP32.png";
        public override string? CustomBigIconPath => "res://TH_Youmu/ArtWorks/Powers/WWFMP64.png";
        protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<StrengthPower>(),HoverTipFactory.FromPower<DexterityPower>(),HoverTipFactory.ForEnergy(this)];
        private int _counter = 0;
        public WindWoodFireMountainPower() { }
        public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
        {
            if (player != base.Owner.Player)
            {
                return;
            }
            _counter++;
             if (_counter > 4)
            {
                _counter = 0;
            }
             this.Flash();
            switch (_counter)
            {
                case 1:
                    await CardPileCmd.Draw(choiceContext,Amount,player);
                    break;
                case 2:
                    await PlayerCmd.GainEnergy(Amount,player);
                    break;
                case 3:
                    await PowerCmd.Apply<StrengthPower>(base.Owner, Amount,base.Owner, null);
                    break;
                case 4:
                    await PowerCmd.Apply<DexterityPower>(base.Owner, Amount,base.Owner, null);
                    break;
                default:
                    break;
            }
        }
    }

}