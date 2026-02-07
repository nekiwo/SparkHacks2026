using Godot;
using System;
using System.Threading.Tasks;

public partial class Transition : ColorRect
{
	private AnimationPlayer _anim = null;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_anim = GetNode<AnimationPlayer>("AnimationPlayer");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public async Task PlayIn()
	{
		_anim.Play("in");
		await ToSignal(GetTree().CreateTimer(_anim.CurrentAnimationLength), "timeout");
	}

	public async Task PlayOut()
	{
		_anim.Play("out");
		await ToSignal(GetTree().CreateTimer(_anim.CurrentAnimationLength), "timeout");
	}
}
