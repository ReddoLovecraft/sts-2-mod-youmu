using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Nodes.Combat;
using System;
using System.Reflection;

public partial class NYoumuEnergyCounter : NEnergyCounter
{
	private const string DarkenedMatPath = "res://materials/ui/energy_orb_dark.tres";
	private const int FrameSizePx = 256;
	private const int Columns = 6;
	private const int Rows = 6;
	private const int FrameCount = Columns * Rows;
	private const float FlipbookFps = 24f;

	private static readonly Color EnergyTint = new Color(0.2f, 1f, 0.8f, 1f);

	private Player? _player;
	private Label? _label;
	private Control? _layers;
	private Control? _rotationLayers;
	private TextureRect? _flipbookRect;
	private Control? _rotationLayer2;
	private Control? _rotationLayer3;
	private AtlasTexture? _flipbookAtlasTexture;
	private Material? _darkenedMat;
	private bool _isEnergyZero;

	private float _flipbookAccumulatorSeconds;
	private int _flipbookFrameIndex;
	private bool _initialized;

	public override void _EnterTree()
	{
	}

	public override void _Ready()
	{
		_player = GetPlayerFromBase();

		_label = GetNodeOrNull<Label>("Label");
		_layers = GetNodeOrNull<Control>("%Layers");
		_rotationLayers = GetNodeOrNull<Control>("%RotationLayers");
		_darkenedMat = GD.Load<Material>(DarkenedMatPath);

		_flipbookRect =
			GetNodeOrNull<TextureRect>("%Layer2") ??
			GetNodeOrNull<TextureRect>("Layers/RotationLayers/Layer2");
		_rotationLayer2 = _flipbookRect;
		_rotationLayer3 = GetNodeOrNull<Control>("Layers/RotationLayers/Layer3");

		var atlas = GD.Load<Texture2D>("res://TH_Youmu/ArtWorks/VFX/EnergySheet.png");
		if (_flipbookRect != null && atlas != null)
		{
			_flipbookAtlasTexture = new AtlasTexture
			{
				Atlas = atlas,
				Region = new Rect2(0, 0, FrameSizePx, FrameSizePx)
			};
			_flipbookRect.Texture = _flipbookAtlasTexture;
			_flipbookRect.Visible = true;
		}

		ApplyEnergyTint(EnergyTint);
		UpdateFlipbookFrame();

		_initialized = true;

		if (_player != null)
		{
			CombatManager.Instance.StateTracker.CombatStateChanged += OnCombatStateChanged;
			_player.PlayerCombatState.EnergyChanged += OnEnergyChanged;
		}

		RefreshLabelSafe();
	}

	public override void _ExitTree()
	{
		if (_player != null)
		{
			CombatManager.Instance.StateTracker.CombatStateChanged -= OnCombatStateChanged;
			_player.PlayerCombatState.EnergyChanged -= OnEnergyChanged;
		}
	}

	public override void _Process(double delta)
	{
		if (_rotationLayers != null)
		{
			var speed = _isEnergyZero ? 5f : 30f;
			for (var i = 0; i < _rotationLayers.GetChildCount(); i++)
			{
				if (_rotationLayers.GetChild(i) is Control c)
				{
					c.RotationDegrees += (float)delta * speed * (i + 1);
				}
			}
		}

		if (_isEnergyZero || _flipbookAtlasTexture == null)
		{
			return;
		}

		_flipbookAccumulatorSeconds += (float)delta;
		var frameTime = 1f / FlipbookFps;
		while (_flipbookAccumulatorSeconds >= frameTime)
		{
			_flipbookAccumulatorSeconds -= frameTime;
			_flipbookFrameIndex = (_flipbookFrameIndex + 1) % FrameCount;
			UpdateFlipbookFrame();
		}
	}

	private void OnCombatStateChanged(CombatState combatState)
	{
		RefreshLabelSafe();
	}

	private void OnEnergyChanged(int oldEnergy, int newEnergy)
	{
		if (!_initialized)
		{
			return;
		}

		RefreshLabelSafe();
	}

	private void RefreshLabelSafe()
	{
		if (!_initialized || _player == null || _label == null || _layers == null || _rotationLayers == null)
		{
			return;
		}

		var playerCombatState = _player.PlayerCombatState;
		_isEnergyZero = playerCombatState.Energy == 0;
		_label.Text = $"{playerCombatState.Energy}/{playerCombatState.MaxEnergy}";

		_layers.Modulate = _isEnergyZero ? Colors.DarkGray : Colors.White;
		_rotationLayers.Modulate = _isEnergyZero ? Colors.DarkGray : Colors.White;
		ApplyEnergyVisualState();
	}

	private void UpdateFlipbookFrame()
	{
		if (_flipbookAtlasTexture == null)
		{
			return;
		}

		var x = (_flipbookFrameIndex % Columns) * FrameSizePx;
		var y = (_flipbookFrameIndex / Columns) * FrameSizePx;
		_flipbookAtlasTexture.Region = new Rect2(x, y, FrameSizePx, FrameSizePx);
	}

	private static Player? GetPlayerFromBaseField(NEnergyCounter counter)
	{
		var field = typeof(NEnergyCounter).GetField("_player", BindingFlags.NonPublic | BindingFlags.Instance);
		return field?.GetValue(counter) as Player;
	}

	private Player? GetPlayerFromBase()
	{
		try
		{
			return GetPlayerFromBaseField(this);
		}
		catch
		{
			return null;
		}
	}

	private void ApplyEnergyTint(Color tint)
	{
		var back = GetNodeOrNull<CanvasItem>("%EnergyVfxBack");
		if (back != null)
		{
			back.Modulate = tint;
		}

		var front = GetNodeOrNull<CanvasItem>("%EnergyVfxFront");
		if (front != null)
		{
			front.Modulate = tint;
		}

		var backGlow = GetNodeOrNull<CanvasItem>("%EnergyVfxBack")?.GetNodeOrNull<CanvasItem>("BackGlow");
		if (backGlow != null)
		{
			backGlow.Modulate = new Color(tint.R, tint.G, tint.B, 0.45f);
		}

		var frontGlow = GetNodeOrNull<CanvasItem>("%EnergyVfxFront")?.GetNodeOrNull<CanvasItem>("FrontGlow");
		if (frontGlow != null)
		{
			frontGlow.Modulate = new Color(tint.R, tint.G, tint.B, 0.35f);
		}
	}

	private void ApplyEnergyVisualState()
	{
		if (_flipbookRect != null)
		{
			_flipbookRect.Visible = !_isEnergyZero;
		}

		if (_layers != null)
		{
			for (var i = 0; i < _layers.GetChildCount(); i++)
			{
				if (_layers.GetChild(i) is Control c)
				{
					c.Material = _isEnergyZero ? _darkenedMat : null;
				}
			}
		}

		if (_rotationLayers != null)
		{
			for (var i = 0; i < _rotationLayers.GetChildCount(); i++)
			{
				if (_rotationLayers.GetChild(i) is Control c)
				{
					c.Material = _isEnergyZero ? _darkenedMat : null;
				}
			}
		}
	}
}
