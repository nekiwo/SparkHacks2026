using Godot;
using System;
using System.Collections.Generic;

public partial class InfoInteractable : InteractableTemplate
{
	[Export]
	public string InfoDialogueFilePath;

	public override async void _DoStuff()
	{
		_Deactivate();
		Dictionary<string, Action> effects = new Dictionary<string, Action>();
		await _dialogueHandler.TriggerDialogue(InfoDialogueFilePath, effects);
		_Activate();
	}
}
