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

	生命粉尘 稀有药
	所有玩家回复35%的生命。

	远古秘药 稀有药
	将你的血量和能量恢复到上限。

	解除烟筒 稀有药
	移除所有玩家的负面效果。

	鬼人粉尘 罕见药
	所有玩家本回合内获得7点力量。

	守护之爪 罕见药
	持有时，在战斗开始时获得2层敏捷。

	力量之爪 罕见药
	持有时，在战斗开始时获得2层力量。

	磨刀鲹鱼的鳍 普通药
	开刃。

	我说a10拿俩护符cos不加费添水有没有懂得。


	事件：满开的西行妖 仅限全员妖梦。回到千年之前，uuz自杀的时候。上前阻拦。失去最大生命值50%的生命。获得西行妖的树枝。无动于衷。获得悔恨。
	事件：食材采购（花钱买各种食物遗物）仅限1，3层事件，且金币在150以上才出。150G，随机获得草莓，芒果，梨子，火龙果，李家华夫饼，营养牡蛎，布制果实，大蘑菇，天选芝士，芒果？？？中的一个。300G，随机所得这10个中的3个。600G，获得除了大蘑菇和芒果？？？之外的8个。
	事件：uuz大人（要食物遗物给删牌/没有掉半条命然后拿钱，真，女鬼索命（））。仅限有人是妖梦。选项1：给所有食物遗物（包含上面10种），然后删除至多等量遗物的牌。选项2：失去50%最大生命值，然后获得600G。
	事件：Give me Yamato观看了两兄弟打斗，每个人获胜概率50%。支持但丁获胜获得：大排档椅。支持Vergil，升级你的所有攻击牌。仅限2，3层。
	事件：非想天則珍贵资料：给我那个能力/技能/攻击三选一 仅限1，2层事件且全员妖梦。
	事件：偶遇铃仙兔兔 仅限有100G且有人是妖梦，不限层。选择第一种药 -100G，回复50%生命值。选择第二种药：-100G，获得10最大生命值。来点其他药水：-100G,获得1普通1罕见1普通药水。

	樱之意志 事件遗物
	战斗开始时，将3张樱花洗入你的抽牌堆中。
	在你的回合开始时，额外获得一点能量。

	大排档椅 事件遗物
	每当你获得来源于你的正面效果时，将效果翻倍。
	经典塑料难降解(什)。你感觉这把椅子很有Power。

	半灵符咒 稀有遗物
	每回合一次，当你被施加负面效果时，免疫这次效果。
	来自冥界的神秘符咒，带在身上可使人保持神智清明。

	樱之结界 稀有遗物
	你单次受到的伤害不会高于9点。
	即使被弹幕击中和按下X也不会炸掉的坚固结界。

	黑色过膝袜 稀有遗物
	现在连续技将在2层以上时才会给予硬直。
	绝对领域绝赞好评中。

	幽灵月饼 稀有遗物
	在你的回合结束时，如果本回合内没有打出过不具有虚无的卡牌，获得1层无实体。
	妖梦特制的冰皮月饼，不知道会不会有半灵的口感呢？

	彼岸花 稀有遗物
	每当敌人死亡时，将你的最大生命值提高1点。
	据说是只开在冥界三途河边、忘川彼岸的接引之花。

	引魂灯 罕见遗物
    在你的回合开始时，将此遗物的计数减少1点，然后获得1点能量并抽1张牌。每当普通/精英/BOSS敌人死亡时，将计数增加1/3/5点。
	一盏可以吸引亡魂的灯，然后燃烧它们用以照明。

	心眼 罕见遗物
	你对具有负面效果的敌人造成的伤害翻倍。
	来自冥界的灵符，在战斗中使用它可以洞悉敌人的弱点。

	幽明之镜 罕见遗物
	每当你触发当身技时，获得1层缓冲。
	镜中倒映出幽明境界的影像，达到心如止水的境界，即可洞察一切。

	光剑 罕见遗物
	右键该遗物可以开关本遗物。当前状态：开/关。开启时，你造成伤害的效果改为使得敌人失去原效果基础值150%的生命。

	备用半灵 商店遗物
	每当你失去生命时，下回合获得等同于失去生命半值的格挡。如果你有半灵的一半，本遗物效果翻倍。
	某神秘市场神的作品之一，这张卡牌保存着魂魄妖梦的能力，能召唤出第二只幽灵。可以和半灵的一半共同使用，这样就真的像妖梦了。
	
	闪避布 普通遗物
	被攻击时有30%概率免受这次伤害。
	遮住身体就能躲避掉弹幕的攻击，原理不明。

	半灵的一半  普通遗物
	每当你使用攻击牌造成伤害时，额外使敌人失去造成伤害25%的生命。如果你有备用半灵，本遗物效果翻倍。
    这不是半个妖梦，而是半个半灵。也就是四分之一。一般来说，被幽灵附身可算不上好事，不过能帮忙攻击的话还算勉强靠谱吧。

	西行妖的花瓣 普通遗物
	在你的回合开始时，给予随机敌人4层消亡。
	樱花树上飘落的花瓣，透着一丝死亡的气息。

	*/
}
