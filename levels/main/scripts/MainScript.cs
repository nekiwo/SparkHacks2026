using Godot;
using System;
using System.Collections.Generic;

public partial class MainScript : Node3D
{
	private DialoguePanel _dialogueHandler = null;
	private Node3D _root = null;

	public override void _Ready()
	{
		_dialogueHandler = GetNode<DialoguePanel>("UI/DialoguePanel");
		_root = GetTree().Root.GetNode<Node3D>("Node3D");

		StartDialogue();  
	}

	public override void _Process(double delta)
	{
		
	}

	private async void StartDialogue()
	{
		Dictionary<string, Action> effects = new Dictionary<string, Action>();
		int rizzScore = 0;

		effects.Add("increaseRizz", async () =>
		{
			rizzScore++;
		});

		 effects.Add("decreaseRizz", async () =>
		{
			rizzScore--;
		});

		effects.Add("WalkScene", async () =>
		{
			_root.GetNode<Node3D>("bedroom").Visible = false;
			_root.GetNode<Node3D>("outside").Visible = true;
			await _dialogueHandler.TriggerDialogue("res://levels/main/dialogue/meet_friend/conversation.json", effects);
		});

		effects.Add("lunchScene", async () =>
		{
			_root.GetNode<Node3D>("outside").Visible = false;
			_root.GetNode<Node3D>("lunch").Visible = true;
			await _dialogueHandler.TriggerDialogue("res://levels/main/dialogue/lunch/conversation.json", effects);
		});

		effects.Add("lectureScene", async () =>
		{
			_root.GetNode<Node3D>("outside").Visible = false;
			_root.GetNode<Node3D>("lecture").Visible = true;
			await _dialogueHandler.TriggerDialogue("res://levels/main/dialogue/lecture/talk.json", effects);
		});

		

		await _dialogueHandler.TriggerDialogue("res://levels/main/dialogue/apartment/bedroom.json", effects);
	}
}
