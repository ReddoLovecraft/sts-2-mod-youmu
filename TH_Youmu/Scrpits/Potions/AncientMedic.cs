using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using System.Collections.Generic;
using System.Threading.Tasks;
using TH_Youmu.Scripts.Main;
using TH_Youmu.Scrpits.Powers;

namespace TH_Youmu.Scrpits.Potions;
[Pool(typeof(YoumuPotionPool))]
public sealed class AncientMedic : CustomPotionModel
{
    public override PotionRarity Rarity => PotionRarity.Rare;

    public override PotionUsage Usage => PotionUsage.CombatOnly;

    public override TargetType TargetType => TargetType.Self;

    public override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.ForEnergy(this)];
    public override string? CustomPackedImagePath => "res://TH_Youmu/ArtWorks/Potions/ANCIENT_MEDIC.png";
    public override string? CustomPackedOutlinePath => "res://TH_Youmu/ArtWorks/Potions/Outlines/ANCIENT_MEDIC.png"; 
     protected override IEnumerable<DynamicVar> CanonicalVars => (new DynamicVar[1]
    {
        new EnergyVar(1)
    });
    protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
    {
        int add=Owner.Creature.MaxHp-Owner.Creature.CurrentHp;
        await CreatureCmd.Heal(Owner.Creature,add);
        await PlayerCmd.SetEnergy(Owner.Creature.Player.MaxEnergy, Owner.Creature.Player);
    }
}
