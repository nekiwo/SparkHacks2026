using System.Collections.Generic;

#nullable enable

public class Dialogue
{
    public string Speaker { get; set; }
    public string? Sprite { get; set; }
    public string Content { get; set; }
    public string? Effect { get; set; }
    public string? Next { get; set; }
    public List<DialogueOption>? Options { get; set; }
}