using BaseLib.Abstracts;
using BaseLib.Patches.Content;
using Godot;
using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Cards;
using TH_Youmu.Scrpits.Cards;
using TH_Youmu.Scrpits.Relics;


namespace TH_Youmu.Scripts.Main
{
	public class YoumuCharacter : PlaceholderCharacterModel
	{
		public override Color NameColor => new Color("17ab8dff");
		public override Color EnergyLabelOutlineColor => new Color("00da7aff");
		public override Color DialogueColor => new Color("069e74ff");
		public override Color MapDrawingColor => new Color("1eb37eff");
		public override Color RemoteTargetingLineColor => new Color("01b78fff");
		public override Color RemoteTargetingLineOutline => new Color("028f71ff");
		public override CharacterGender Gender => CharacterGender.Feminine;
		public override int StartingHp => 71;
		public override string CustomVisualPath => "res://TH_Youmu/ArtWorks/Character/youmu.tscn";
		public override string CustomTrailPath => "res://TH_Youmu/ArtWorks/VFX/YoumuCardTrail.tscn";
		public override string CustomIconTexturePath => "res://TH_Youmu/ArtWorks/Character/youmu_icon.png";
		public override string CustomIconPath => "res://TH_Youmu/ArtWorks/Character/youmu_icon.tscn";
		public override string CustomEnergyCounterPath => "res://TH_Youmu/ArtWorks/VFX/youmu_energy_counter.tscn";
		// // 篝火休息动画。
		public override string CustomRestSiteAnimPath => "res://TH_Youmu/ArtWorks/Character/youmurest.tscn";
		// // 商店人物动画。
		public override string CustomMerchantAnimPath => "res://TH_Youmu/ArtWorks/Character/youmu_merchant.tscn";
		public override string CustomArmPointingTexturePath => "res://TH_Youmu/ArtWorks/Character/multiplayer_hand_youmu_point.png";
		public override string CustomArmRockTexturePath => "res://TH_Youmu/ArtWorks/Character/multiplayer_hand_youmu_rock.png";
		public override string CustomArmPaperTexturePath => "res://TH_Youmu/ArtWorks/Character/multiplayer_hand_youmu_paper.png";
		public override string CustomArmScissorsTexturePath => "res://TH_Youmu/ArtWorks/Character/multiplayer_hand_youmu_scissors.png";
		public override string CustomCharacterSelectBg => "res://TH_Youmu/ArtWorks/Character/Youmu_bg.tscn";
		public override string CustomCharacterSelectIconPath => "res://TH_Youmu/ArtWorks/Character/char_select_youmu.png";
		public override string CustomCharacterSelectLockedIconPath => "res://TH_Youmu/ArtWorks/Character/char_select_youmu_locked.png";
		public override string CustomCharacterSelectTransitionPath => "res://materials/transitions/silent_transition_mat.tres";
		public override string CustomMapMarkerPath => "res://TH_Youmu/ArtWorks/Character/map_marker_youmu.png";
		// 攻击音效
		public override string CustomAttackSfx => YoumuInit.ToModSfxPath("TH_Youmu/ArtWorks/SFX/attack.wav");
		// // 施法音效
		public override string CustomCastSfx => YoumuInit.ToModSfxPath("TH_Youmu/ArtWorks/SFX/cast.wav");
		// // 死亡音效
		public override string CustomDeathSfx => YoumuInit.ToModSfxPath("TH_Youmu/ArtWorks/SFX/die.ogg");
		public override string CharacterSelectSfx  => YoumuInit.ToModSfxPath("TH_Youmu/ArtWorks/SFX/characterselect.wav");
		public override string CharacterTransitionSfx => YoumuInit.ToModSfxPath("TH_Youmu/ArtWorks/SFX/transition.wav");
		public override CardPoolModel CardPool => ModelDb.CardPool<YoumuCardPool>();
		public override RelicPoolModel RelicPool => ModelDb.RelicPool<YoumuRelicPool>();
		public override PotionPoolModel PotionPool => ModelDb.PotionPool<YoumuPotionPool>();

		// 初始卡组
		public override IEnumerable<CardModel> StartingDeck => [
			ModelDb.Card<Strike>(),
			ModelDb.Card<Strike>(),
			ModelDb.Card<Strike>(),
			ModelDb.Card<Strike>(),
			ModelDb.Card<Defend>(),
			ModelDb.Card<Defend>(),
			ModelDb.Card<Defend>(),
			ModelDb.Card<Defend>(),
	];

		// 初始遗物
		public override IReadOnlyList<RelicModel> StartingRelics => [
			ModelDb.Relic<RightSword>()
	];

		// 攻击建筑师的攻击特效列表
		public override List<string> GetArchitectAttackVfx() => [
		"vfx/vfx_attack_slash",
        "vfx_starry_impact",
        "vfx/vfx_giant_horizontal_slash",
		"vfx/vfx_attack_slash",
        "vfx_starry_impact",
        "vfx/vfx_giant_horizontal_slash",
		"vfx/vfx_attack_slash",
        "vfx_starry_impact",
        "vfx/vfx_giant_horizontal_slash",
		"vfx/vfx_attack_slash"
		];
		public override CreatureAnimator GenerateAnimator(MegaSprite controller)
		{
			AnimState animState = new AnimState("idle", isLooping: true);
			AnimState animState2 = new AnimState("cast");
			AnimState animState3 = new AnimState("attack");
			AnimState animState4 = new AnimState("hit");
			AnimState state = new AnimState("die");
			AnimState animState5 = new AnimState("relaxed_loop", isLooping: true);
			AnimState animState6 = new AnimState("guard");
			animState2.NextState = animState;
			animState3.NextState = animState;
			animState4.NextState = animState;
			animState6.NextState = animState;
			animState5.AddBranch("Idle", animState);
			CreatureAnimator creatureAnimator = new CreatureAnimator(animState, controller);
			creatureAnimator.AddAnyState("Idle", animState);
			creatureAnimator.AddAnyState("Dead", state);
			creatureAnimator.AddAnyState("Hit", animState4);
			creatureAnimator.AddAnyState("Attack", animState3);
			creatureAnimator.AddAnyState("Cast", animState2);
			creatureAnimator.AddAnyState("Guard", animState6);
			creatureAnimator.AddAnyState("relaxed_loop", animState5);		
			return creatureAnimator;
		}
        //卡牌设计，妖梦
        /*
		苍天形态
		3→2c金能力
		固有
		每当你取消时，获得等同于被取消的那张牌的耗能的能量并重置连段计数。
		现在你的取消将无视取消等级直接成功。
		获得等同于造成的伤害层剑气。

		半分虚幻的园艺师
		2->1c金能力
		虚无。
		带有虚无的卡牌可以被免费打出。

		幽人之庭师
		1->0c金能力
		每当你打出带有虚无的卡牌时，派生其对应类型的卡牌1，并为被派生的卡牌添加虚无。

		广有射鸟怪事
		2->1c金能力
		在你的回合开始时，将一张被升级过的带有虚无和消耗的随机攻击牌放入所有你的手中。

		半人半灵的半吊子
		1->0c蓝能力
		每当你派生时，将被派生的卡牌自动打出1次并将1张笨拙洗入你的抽牌堆。

		切舍御免
		2->1c金能力
		每当你失去生命时，将你在本场战斗中造成的伤害提高1倍。

		趁势回锋
		1c蓝能力
		+固有
		触发当身技后，将卡牌重新放回你的手中。		
		
		六道怪奇
		2->1c蓝能力
		每当你打出和上一张卡牌不同类型的卡牌时，获得E。
		
		卡牌斩断者 1c蓝能力
		+固有
		在你的回合开始时，选择至多1张牌消耗，然后获得等同于这张牌基础耗能的E。

		半身剑锋
		1c蓝能力
		在你的回合开始时，获得3->5层剑气。

		半身剑胆
		1->0c金能力
		你的剑气不会在回合结束时被移除。

		半人半灵的庭师
		2->1c金能力
		每当你派生时，获得等同于被派生的那张牌的耗能的能量。


		瞑斩「楼观予我心之眼」
		1->0c蓝能力
		每当你打出攻击牌时，攻击派生2。
		
		
		明镜止水
		1c蓝能力
		每有一层连段，造成的伤害增加10%-20%。

		死欲半灵
		3->2c能力
		每当敌人受到来源于你的伤害时，获得等量消亡。

		生与死的Half&Half
		2c金能力
		虚无(-)
		当你的生命超过50%时，造成的伤害翻倍，每打出一张攻击牌就失去6点生命。(instanced)
		当你的生命低于50%时，受到的伤害减半。每打出一张技能牌就回复6点生命。虚无(-)
		
		二天一流
		2->1c金能力
		你的攻击牌造成的伤害减少25%，但在打出时将被额外打出一次。(instanced,Single)
		如果你有五轮书，则不减少伤害。
		
		风林火山
		1->0c蓝能力
		固有
		在你的回合开始时，轮替获得以下效果：在你的回合开始额外抽1张牌。\n在你的回合开始时获得1点能量。\n在你的回合开始时获得1点力量。\n在你的回合开始时获得1点敏捷。

        剑心通明 
		3->2c金能力
		固有
		在你的回合开始时，如果所有敌人的意图中有防御，每有一个就获得1层力量。\n如果所有敌人的意图中有攻击，每有一个就获得1层敏捷。\n如果所有敌人的意图中有其他，每有一个就获得E并抽1张牌。

		事件：食材采购（+食物遗物）
		事件：uuz大人（要食物遗物给删牌/直接拿钱）

		触发当身技后，获得1层缓冲。遗物吧

        
		
		*/
    }
}
