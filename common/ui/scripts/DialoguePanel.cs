using Godot;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

public partial class DialoguePanel : PanelContainer
{
	private AnimationPlayer _dialogueAnim = null;
	private RichTextLabel _dialogueText = null;
	private RichTextLabel _speakerText = null;
	private VBoxContainer _dialogueOptions = null;
	private Transition _transition = null;
	// private PlayerControl _player = null;

	private StringName _interact = new StringName("interact");
	private bool _skipToggle = false;

	public async Task TriggerDialogue(string dialoguePath, Dictionary<string, Action> effects, bool playAnimations = true)
	{
		await _transition.PlayOut();
		GD.Print("triggering " + dialoguePath);
		// _player.Locked = true;

		// Load and deserialize dialogue
		FileAccess dialogueFile = FileAccess.Open(dialoguePath, FileAccess.ModeFlags.Read);
		if (dialogueFile == null) GD.PushError(FileAccess.GetOpenError());
		string uncastedDialogue = dialogueFile.GetAsText();
		Dictionary<string, Dialogue> dialogueChain = JsonSerializer.Deserialize<Dictionary<string, Dialogue>>(uncastedDialogue);

		// Load first speaker info
		_dialogueText.VisibleCharacters = 0;
		_dialogueText.Text = dialogueChain["Start"].Content;
		_speakerText.Text = "[b]" + dialogueChain["Start"].Speaker + "[/b]";

		// Pop-up animation trigger
		if (playAnimations)
		{
			_dialogueAnim.Play("PopUp");
			await ToSignal(GetTree().CreateTimer(_dialogueAnim.CurrentAnimationLength), "timeout");
		}

		// For each dialogue
		Dialogue currDialogue = dialogueChain["Start"];
		while (currDialogue != null)
		{
			// Update info
			_dialogueText.VisibleCharacters = 0;
			_dialogueText.Text = currDialogue.Content;
			_speakerText.Text = "[b]" + currDialogue.Speaker + "[/b]";

			await _RevealText(currDialogue.Content); // Text animation
			// TODO: show continue icon
			await _KeyboardInput(_interact); // Wait for button

			// Execute effect
			GD.Print("EFFECT: " + currDialogue.Effect);
			if (currDialogue.Effect != "" && currDialogue.Effect != "increaseRizz" && currDialogue.Effect != "decreaseRizz")
			{
				GD.Print("exiting at l60");
				_dialogueAnim.Play("PopDown");
				await ToSignal(GetTree().CreateTimer(1.0), "timeout");
				await _transition.PlayIn();
				effects[currDialogue.Effect]();
				return;
			}

			if (currDialogue.Effect == "increaseRizz" || currDialogue.Effect == "decreaseRizz")
			{
				effects[currDialogue.Effect]();
			}
			
			if (currDialogue.Options == null || currDialogue.Options.Count == 0)
			{
				GD.Print("continueing at l68");
				currDialogue = dialogueChain[currDialogue.Next];
				continue;
			}

			// Show options
			_speakerText.Visible = false;
			_dialogueText.Visible = false;
			_dialogueOptions.Visible = true;
			for (int j = 0; j < currDialogue.Options.Count; j++)
			{
				if (currDialogue.Options[j] == null || currDialogue.Options[j].Content == "")
				{
					continue;
				}
				RichTextLabel optionText = _GetOption(j);
				optionText.Text = "- " + currDialogue.Options[j].Content;
				optionText.AddThemeColorOverride("default_color", new Color("5e5e5e"));
				optionText.Visible = true;
			}
			_GetOption(0).AddThemeColorOverride("default_color", new Color("ffffff"));

			// Wait for response
			int selectedOption = await _OptionInput(currDialogue.Options.Count);
			string next = currDialogue.Options[selectedOption].Next;
			if (next != "STOP")
			{

				_speakerText.Visible = true;
				_dialogueText.Visible = true;
				_dialogueOptions.Visible = false;
				for (int j = 0; j < currDialogue.Options.Count; j++)
				{
					_GetOption(j).Visible = false;
				}
			} else
			{
				break;
			}
			Dialogue nextDialogue = dialogueChain[next];
			
			// Open new dialogue and continue
			currDialogue = nextDialogue;
		}

		// Closing animation trigger
		if (playAnimations)
		{
			_dialogueAnim.Play("PopDown");
			await ToSignal(GetTree().CreateTimer(1.0), "timeout");
		}


		_speakerText.Visible = true;
		_dialogueText.Visible = true;
		_dialogueOptions.Visible = false;
		if (currDialogue.Options != null)
		{
			for (int j = 0; j < currDialogue.Options.Count; j++)
			{
				_GetOption(j).Visible = false;
			}
		}

		if (currDialogue.Effect != "")
		{
			await _transition.PlayIn();
			effects[currDialogue.Effect]();
			return;
		} 

		await _transition.PlayIn();

		// _player.Locked = false;
	}

	private async Task _RevealText(string content)
	{
		_skipToggle = false;
		_dialogueText.VisibleCharacters = 0;
		int soundCounter = 0;

		for (int i = 0; i <= content.Length; i++)
		{
			if (_skipToggle)
			{
				_dialogueText.Text = $"{content.Replace(@"|", "")}";
				_dialogueText.VisibleCharacters = content.Length;
				await ToSignal(GetTree(), "process_frame");
				break;
			}
			else if (i > 1)
			{
				// Adds wave to the last character
				_dialogueText.Text = _RevealTextAnimation(content, _dialogueText.VisibleCharacters);
			}

			if (i != content.Length && content[i] == '|')
			{
				content = content.Substring(0, i) + content.Substring(i + 1, content.Length - i - 1);
				_dialogueText.Text = $"{content}";
				await ToSignal(GetTree().CreateTimer(0.4), "timeout");
			}

			_dialogueText.VisibleCharacters++;

			// Play sound
			soundCounter++;
			if (soundCounter == 3)
			{
				GetNode<AudioStreamPlayer>("PopSound").Play();
				soundCounter = 0;
			}

			await ToSignal(GetTree().CreateTimer(0.04), "timeout");
		}
	}

	private string _RevealTextAnimation(string text, int charNum)
	{
		if (text.Length == charNum)
		{
			return $"{text}";
		}
		var wavingChars = text.Substr(charNum, text.Length - charNum);
		// return $"[b]{text[..charNum]}[wave amp=100.0 freq=0.0 connected=1]{wavingChars}[/wave][/b]";
		return $"{text[..charNum]}{wavingChars}";
	}

	private async Task _KeyboardInput(StringName action)
	{
		while (!Input.IsActionJustReleased(action))
		{
			await ToSignal(GetTree(), "process_frame");
		}
		await ToSignal(GetTree(), "process_frame");
		return;
	}

	private RichTextLabel _GetOption(int optionIndex)
	{
		return _dialogueOptions.GetNode<RichTextLabel>("Option" + optionIndex);
	}

	private async Task<int> _OptionInput(int optionCount)
	{
		int currentOption = 0;

		while (!Input.IsActionJustReleased(_interact))
		{
			if (Input.IsActionJustReleased(new StringName("move_up")) && currentOption > 0)
			{
				_GetOption(currentOption).AddThemeColorOverride("default_color", new Color("5e5e5e"));
				currentOption--;
				_GetOption(currentOption).AddThemeColorOverride("default_color", new Color("ffffff"));

				GetNode<AudioStreamPlayer>("MechSound").Play();
			}

			if (Input.IsActionJustReleased(new StringName("move_down")) && currentOption < optionCount - 1)
			{
				_GetOption(currentOption).AddThemeColorOverride("default_color", new Color("5e5e5e"));
				currentOption++;
				_GetOption(currentOption).AddThemeColorOverride("default_color", new Color("ffffff"));

				GetNode<AudioStreamPlayer>("MechSound").Play();
			}

			await ToSignal(GetTree(), "process_frame");
		}

		return currentOption;
	}

	public override void _Ready()
	{
		_dialogueAnim = GetNode<AnimationPlayer>("DialogueAnim");
		_dialogueText = GetNode<RichTextLabel>("Div/Content");
		_dialogueOptions = GetNode<VBoxContainer>("Div/Options");
		_speakerText = GetNode<RichTextLabel>("Div/Speaker");
		_transition = GetNode<Transition>("../Transition");
		// _player = GetNode<PlayerControl>("../../Player");
	}

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionReleased(_interact))
		{
			_skipToggle = true;
		}
	}
}
