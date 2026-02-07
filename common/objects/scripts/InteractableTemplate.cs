using Godot;
using System;

public abstract partial class InteractableTemplate : Area3D
{
	private StringName _playerGroup = new StringName("player");
	private StringName _interactInput = new StringName("interact");
	private TextureRect _interactIcon = null;
	protected PlayerControl _playerNode = null;
	protected DialoguePanel _dialogueHandler = null;
	public bool CanInteract = false;
	public bool IsFinished = false;

	public override void _Ready()
	{
		_interactIcon = GetTree().Root.GetNode<TextureRect>("Node3D/UI/InteractIcon");
		_dialogueHandler = GetTree().Root.GetNode<DialoguePanel>("Node3D/UI/DialoguePanel");
		_Deactivate();
	}

	private void _on_body_entered(PhysicsBody3D body) {
		if (body.IsInGroup(_playerGroup) && !IsFinished) {
			_playerNode = (PlayerControl)body;
			_Activate();
		}
	}
	private void _on_body_exited(PhysicsBody3D body)
	{
		if (body.IsInGroup(_playerGroup)) {
			_Deactivate();
		}
	}

	public void _Activate() {
		CanInteract = true;
		_interactIcon.Visible = true;
	}

	public void _Deactivate() {
		CanInteract = false;
		_interactIcon.Visible = false;
	}

	public override void _Input(InputEvent @event) {
		if (CanInteract && !IsFinished && @event.IsActionPressed(_interactInput) && !_playerNode.Locked) {
			_DoStuff();
		}
	}

	public abstract void _DoStuff();
}
