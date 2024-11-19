using System;

[Serializable]
public class Artifact
{
    public string gptPrompt { get; set; }
    public string originalPrompt { get; set; }
    public string imageURL { get; set; }
    public string videoURL { get; set; }
}
