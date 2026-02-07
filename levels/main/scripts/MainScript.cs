using Godot;
using System;
using System.Collections.Generic;

public partial class MainScript : Node3D
{
	private DialoguePanel _dialogueHandler = null;

	public override void _Ready()
	{
		_dialogueHandler = GetNode<DialoguePanel>("UI/DialoguePanel");

		// StartDialogue();   
	}

	public override void _Process(double delta)
	{
		
	}

	private async void StartDialogue()
	{
		Dictionary<string, Action> effects = new Dictionary<string, Action>();
		await _dialogueHandler.TriggerDialogue("res://levels/main/dialogue/example.json", effects);
	}
}
