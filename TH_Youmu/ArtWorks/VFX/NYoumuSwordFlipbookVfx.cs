using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MegaCrit.Sts2.Core.Nodes.Vfx;

public partial class NYoumuSwordFlipbookVfx : Node2D
{
	private const string AdditiveMatPath = "res://TH_Youmu/ArtWorks/VFX/canvas_item_material_additive_shared.tres";

	private enum AnchorMode
	{
		World,
		SourceCenter,
		SourceFront,
		TargetCenter,
		TargetFront,
		TargetHead
	}

	private enum MotionMode
	{
		None,
		ToTargetCenter
	}

	private readonly record struct Preset(
		string FramesDir,
		AnchorMode Anchor,
		MotionMode Motion,
		float BaseSpeed,
		float RotationDegPerSec,
		float Scale,
		Vector2 Offset,
		float FrontOffsetPx,
		float HeadOffsetPx
	);

	private static readonly Dictionary<string, Preset> Presets = new(StringComparer.Ordinal)
	{
		["circlesword"] = new(
			FramesDir: "res://TH_Youmu/ArtWorks/VFX/circlesword",
			Anchor: AnchorMode.World,
			Motion: MotionMode.None,
			BaseSpeed: 1f,
			RotationDegPerSec: 360f,
			Scale: 1f,
			Offset: Vector2.Zero,
			FrontOffsetPx: 0f,
			HeadOffsetPx: 0f
		),
		["down_sword"] = new(
			FramesDir: "res://TH_Youmu/ArtWorks/VFX/down_sword",
			Anchor: AnchorMode.TargetHead,
			Motion: MotionMode.None,
			BaseSpeed: 1f,
			RotationDegPerSec: 0f,
			Scale: 1f,
			Offset: Vector2.Zero,
			FrontOffsetPx: 0f,
			HeadOffsetPx: 0f
		),
		["life_death_sword"] = new(
			FramesDir: "res://TH_Youmu/ArtWorks/VFX/life_death_sword",
			Anchor: AnchorMode.SourceFront,
			Motion: MotionMode.ToTargetCenter,
			BaseSpeed: 1f,
			RotationDegPerSec: 0f,
			Scale: 1f,
			Offset: Vector2.Zero,
			FrontOffsetPx: 140f,
			HeadOffsetPx: 0f
		),
		["normal_sword"] = new(
			FramesDir: "res://TH_Youmu/ArtWorks/VFX/normal_sword",
			Anchor: AnchorMode.TargetFront,
			Motion: MotionMode.None,
			BaseSpeed: 1f,
			RotationDegPerSec: 0f,
			Scale: 1f,
			Offset: Vector2.Zero,
			FrontOffsetPx: 110f,
			HeadOffsetPx: 0f
		),
		["pink_sword"] = new(
			FramesDir: "res://TH_Youmu/ArtWorks/VFX/pink_sword",
			Anchor: AnchorMode.SourceFront,
			Motion: MotionMode.None,
			BaseSpeed: 1f,
			RotationDegPerSec: 0f,
			Scale: 1f,
			Offset: Vector2.Zero,
			FrontOffsetPx: 120f,
			HeadOffsetPx: 0f
		),
		["reflect_sword"] = new(
			FramesDir: "res://TH_Youmu/ArtWorks/VFX/reflect_sword",
			Anchor: AnchorMode.SourceFront,
			Motion: MotionMode.None,
			BaseSpeed: 1f,
			RotationDegPerSec: 0f,
			Scale: 1f,
			Offset: Vector2.Zero,
			FrontOffsetPx: 120f,
			HeadOffsetPx: 0f
		),
		["rise_up_sword"] = new(
			FramesDir: "res://TH_Youmu/ArtWorks/VFX/rise_up_sword",
			Anchor: AnchorMode.TargetFront,
			Motion: MotionMode.None,
			BaseSpeed: 1f,
			RotationDegPerSec: 0f,
			Scale: 1f,
			Offset: Vector2.Zero,
			FrontOffsetPx: 110f,
			HeadOffsetPx: 0f
		),
		["vfx_bluesword"] = new(
			FramesDir: "res://TH_Youmu/ArtWorks/VFX/vfx_bluesword",
			Anchor: AnchorMode.SourceFront,
			Motion: MotionMode.None,
			BaseSpeed: 1f,
			RotationDegPerSec: 0f,
			Scale: 3.6f,
			Offset: new Vector2(260f, 0f),
			FrontOffsetPx: 120f,
			HeadOffsetPx: 0f
		),
		["vfx_green_sword"] = new(
			FramesDir: "res://TH_Youmu/ArtWorks/VFX/vfx_green_sword",
			Anchor: AnchorMode.SourceFront,
			Motion: MotionMode.ToTargetCenter,
			BaseSpeed: 1f,
			RotationDegPerSec: 0f,
			Scale: 1f,
			Offset: Vector2.Zero,
			FrontOffsetPx: 140f,
			HeadOffsetPx: 0f
		),
		["vfx_green_sword_gas"] = new(
			FramesDir: "res://TH_Youmu/ArtWorks/VFX/vfx_green_sword_gas",
			Anchor: AnchorMode.SourceFront,
			Motion: MotionMode.ToTargetCenter,
			BaseSpeed: 1f,
			RotationDegPerSec: 0f,
			Scale: 1f,
			Offset: Vector2.Zero,
			FrontOffsetPx: 140f,
			HeadOffsetPx: 0f
		)
	};

	private static readonly Dictionary<string, Texture2D[]> FramesCache = new(StringComparer.Ordinal);
	private static Material? _additiveMat;

	[Export] public float Speed { get; set; } = 1f;
	[Export] public float Fps { get; set; } = 24f;

	private Sprite2D? _sprite;
	private Preset _preset;
	private Texture2D[] _frames = Array.Empty<Texture2D>();
	private float _animTimeScaledSeconds;
	private float _animTotalScaledSeconds;

	private Creature? _source;
	private Creature? _target;
	private Vector2? _worldPos;

	private Vector2 _startPos;
	private Vector2 _endPos;
	private bool _hasInit;

	public static NYoumuSwordFlipbookVfx? Create(string scenePath, Creature? source = null, Creature? target = null, float speed = 1f, Vector2? worldPos = null)
	{
		PackedScene? ps = ResourceLoader.Load<PackedScene>(scenePath);
		if (ps == null)
		{
			return null;
		}

		var inst = ps.Instantiate<Node>();
		if (inst is not NYoumuSwordFlipbookVfx vfx)
		{
			inst.QueueFree();
			return null;
		}

		vfx.Speed = speed;
		vfx._source = source;
		vfx._target = target;
		vfx._worldPos = worldPos;
		return vfx;
	}

	public override void _Ready()
	{
		_sprite = GetNodeOrNull<Sprite2D>("Sprite");
		if (_sprite == null)
		{
			_sprite = new Sprite2D { Name = "Sprite", Centered = true };
			AddChild(_sprite);
		}

		_additiveMat ??= ResourceLoader.Load<Material>(AdditiveMatPath);
		if (_additiveMat != null)
		{
			_sprite.Material = _additiveMat;
		}

		if (!Presets.TryGetValue(Name, out _preset))
		{
			_preset = new Preset(
				FramesDir: "res://TH_Youmu/ArtWorks/VFX",
				Anchor: AnchorMode.World,
				Motion: MotionMode.None,
				BaseSpeed: 1f,
				RotationDegPerSec: 0f,
				Scale: 1f,
				Offset: Vector2.Zero,
				FrontOffsetPx: 120f,
				HeadOffsetPx: 0f
			);
		}

		_sprite.Scale = Vector2.One * _preset.Scale;
		_frames = LoadFrames(_preset.FramesDir);
		_animTotalScaledSeconds = _frames.Length <= 0 ? 0f : _frames.Length / Mathf.Max(1f, Fps);

		InitIfNeeded();
		UpdateFrame(0);
	}

	public override void _Process(double delta)
	{
		if (_frames.Length <= 0 || _sprite == null)
		{
			QueueFree();
			return;
		}

		InitIfNeeded();

		float d = (float)delta;
		float effectiveSpeed = Mathf.Max(0.001f, Speed) * Mathf.Max(0.001f, _preset.BaseSpeed);
		_animTimeScaledSeconds += d * effectiveSpeed;

		int frameIndex = Mathf.FloorToInt(_animTimeScaledSeconds * Mathf.Max(1f, Fps));
		if (frameIndex >= _frames.Length)
		{
			QueueFree();
			return;
		}

		UpdateFrame(frameIndex);

		float progress = _animTotalScaledSeconds <= 0f ? 1f : Mathf.Clamp(_animTimeScaledSeconds / _animTotalScaledSeconds, 0f, 1f);
		if (_preset.Motion == MotionMode.ToTargetCenter)
		{
			GlobalPosition = _startPos.Lerp(_endPos, progress);
		}

		if (_preset.RotationDegPerSec != 0f)
		{
			Rotation += Mathf.DegToRad(_preset.RotationDegPerSec) * d * effectiveSpeed;
		}
	}

	private void InitIfNeeded()
	{
		if (_hasInit)
		{
			return;
		}

		Vector2 anchorPos = ResolveAnchorPosition(_preset.Anchor, _source, _target, _worldPos);
		GlobalPosition = anchorPos + _preset.Offset;

		_startPos = GlobalPosition;
		_endPos = _startPos;
		if (_preset.Motion == MotionMode.ToTargetCenter)
		{
			Vector2 targetCenter = ResolveCreatureCenter(_target) ?? _startPos;
			_endPos = targetCenter;
		}

		_hasInit = true;
	}

	private void UpdateFrame(int frameIndex)
	{
		if (_sprite == null)
		{
			return;
		}

		Texture2D tex = _frames[Mathf.Clamp(frameIndex, 0, _frames.Length - 1)];
		if (!ReferenceEquals(_sprite.Texture, tex))
		{
			_sprite.Texture = tex;
		}
	}

	private Vector2 ResolveAnchorPosition(AnchorMode anchor, Creature? source, Creature? target, Vector2? worldPos)
	{
		switch (anchor)
		{
			case AnchorMode.World:
				return worldPos ?? GlobalPosition;
			case AnchorMode.SourceCenter:
				return ResolveCreatureCenter(source) ?? (worldPos ?? GlobalPosition);
			case AnchorMode.SourceFront:
				return (ResolveCreatureCenter(source) ?? (worldPos ?? GlobalPosition)) + ResolveFrontOffset(source, _preset.FrontOffsetPx);
			case AnchorMode.TargetCenter:
				return ResolveCreatureCenter(target) ?? (worldPos ?? GlobalPosition);
			case AnchorMode.TargetFront:
				return (ResolveCreatureCenter(target) ?? (worldPos ?? GlobalPosition)) + ResolveFrontOffset(target, _preset.FrontOffsetPx);
			case AnchorMode.TargetHead:
				return ResolveCreatureHead(target, _preset.HeadOffsetPx) ?? (worldPos ?? GlobalPosition);
			default:
				return worldPos ?? GlobalPosition;
		}
	}

	private static Vector2 ResolveFrontOffset(Creature? creature, float amountPx)
	{
		if (creature == null)
		{
			return new Vector2(amountPx, 0f);
		}

		try
		{
			float sign = creature.Side == CombatSide.Player ? 1f : -1f;
			return new Vector2(sign * amountPx, 0f);
		}
		catch
		{
			return new Vector2(amountPx, 0f);
		}
	}

	private static Vector2? ResolveCreatureCenter(Creature? creature)
	{
		if (creature == null)
		{
			return null;
		}

		Node2D? n = TryGetCreatureNode2D(creature);
		if (n == null)
		{
			return null;
		}

		Marker2D? marker = n.GetNodeOrNull<Marker2D>("%CenterPos") ?? n.GetNodeOrNull<Marker2D>("CenterPos");
		if (marker != null)
		{
			return marker.GlobalPosition;
		}

		return n.GlobalPosition;
	}

	private static Vector2? ResolveCreatureHead(Creature? creature, float extraUpPx)
	{
		if (creature == null)
		{
			return null;
		}

		Node2D? n = TryGetCreatureNode2D(creature);
		if (n == null)
		{
			return null;
		}

		Marker2D? marker = n.GetNodeOrNull<Marker2D>("%IntentPos") ?? n.GetNodeOrNull<Marker2D>("IntentPos");
		if (marker != null)
		{
			return marker.GlobalPosition + new Vector2(0f, -extraUpPx);
		}

		return n.GlobalPosition + new Vector2(0f, -120f - extraUpPx);
	}

	private static Node2D? TryGetCreatureNode2D(Creature creature)
	{
		object o = creature;
		if (o is Node2D n2d)
		{
			return n2d;
		}

		if (o is Node node && node is Node2D n2d2)
		{
			return n2d2;
		}

		Type t = creature.GetType();
		const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

		foreach (PropertyInfo p in t.GetProperties(flags))
		{
			if (!typeof(Node).IsAssignableFrom(p.PropertyType))
			{
				continue;
			}

			object? v;
			try
			{
				v = p.GetValue(creature);
			}
			catch
			{
				continue;
			}

			if (v is Node2D n)
			{
				return n;
			}
		}

		foreach (FieldInfo f in t.GetFields(flags))
		{
			if (!typeof(Node).IsAssignableFrom(f.FieldType))
			{
				continue;
			}

			object? v;
			try
			{
				v = f.GetValue(creature);
			}
			catch
			{
				continue;
			}

			if (v is Node2D n)
			{
				return n;
			}
		}

		return null;
	}

	private static Texture2D[] LoadFrames(string framesDir)
	{
		if (FramesCache.TryGetValue(framesDir, out Texture2D[]? cached))
		{
			return cached;
		}

		var textures = new List<Texture2D>();
		DirAccess? dir = DirAccess.Open(framesDir);
		if (dir == null)
		{
			FramesCache[framesDir] = Array.Empty<Texture2D>();
			return FramesCache[framesDir];
		}

		string[] files = dir.GetFiles().Where(f => f.EndsWith(".png", StringComparison.OrdinalIgnoreCase)).OrderBy(f => f, StringComparer.Ordinal).ToArray();
		foreach (string f in files)
		{
			string path = $"{framesDir}/{f}";
			Texture2D? t = ResourceLoader.Load<Texture2D>(path);
			if (t != null)
			{
				textures.Add(t);
			}
		}

		FramesCache[framesDir] = textures.ToArray();
		return FramesCache[framesDir];
	}
}
