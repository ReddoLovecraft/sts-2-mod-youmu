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
public sealed class HealthPowder : CustomPotionModel
{
    public override PotionRarity Rarity => PotionRarity.Rare;

    public override PotionUsage Usage => PotionUsage.AnyTime;

    public override TargetType TargetType => TargetType.AllAllies;

    public override string? CustomPackedImagePath => "res://TH_Youmu/ArtWorks/Potions/HEALTH_POWDER.png";
    public override string? CustomPackedOutlinePath => "res://TH_Youmu/ArtWorks/Potions/Outlines/HEALTH_POWDER.png"; 
    protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
    {
        IEnumerable<Creature> enumerable = from c in Owner.Creature.CombatState.GetTeammatesOf(base.Owner.Creature)
			where c != null && c.IsAlive && c.IsPlayer
			select c;
		foreach (Creature item in enumerable)
		{
            int num=35*item.MaxHp/100;
            await CreatureCmd.Heal(item,num);
        }
    }
}
