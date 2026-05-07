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
		public override int StartingHp => 69;
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
			ModelDb.Card<AheadSlash>(),
			ModelDb.Card<AttackWithDefend>()
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
    }
	/*
	遗物，药水，事件：


	事件：满开的西行妖 仅限全员妖梦。回到千年之前，uuz自杀的时候。上前阻拦。失去最大生命值50%的生命。获得西行妖的树枝。无动于衷。获得悔恨。
	事件：食材采购（花钱买各种食物遗物）仅限1，3层事件，且金币在150以上才出。150G，随机获得草莓，芒果，梨子，火龙果，李家华夫饼，营养牡蛎，布制果实，大蘑菇，天选芝士，芒果？？？中的一个。300G，随机所得这10个中的3个。600G，获得除了大蘑菇和芒果？？？之外的8个。
	事件：uuz大人（要食物遗物给删牌/没有掉半条命然后拿钱，真，女鬼索命（））。仅限有人是妖梦。选项1：给所有食物遗物（包含上面10种），然后删除至多等量遗物的牌。选项2：失去50%最大生命值，然后获得600G。
	事件：Give me Yamato观看了两兄弟打斗，每个人获胜概率50%。支持但丁获胜获得：大排档椅。支持Vergil，升级你的所有攻击牌。仅限2，3层。
	事件：非想天則珍贵资料：给我那个能力/技能/攻击三选一 仅限1，2层事件且全员妖梦。
	事件：偶遇铃仙兔兔 仅限有100G且有人是妖梦，不限层。选择第一种药 -100G，回复50%生命值。选择第二种药：-100G，获得10最大生命值。来点其他药水：-100G,获得1普通1罕见1普通药水。


	*/
}
