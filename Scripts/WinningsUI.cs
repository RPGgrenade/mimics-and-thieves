using Godot;
using System;

public partial class WinningsUI : CanvasLayer
{
	[Export] public Control Focus;

    [Export(PropertyHint.Dir)] public string RetryScene;
    [Export(PropertyHint.Dir)] public string MainMenuScene;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		Focus.GrabFocus();
    }

    public void Retry()
    {
        GD.Print("Retrying level: " + (RetryScene != null));
        if (RetryScene != null)
        {
            CarryData.Instance.PlayIntro = false;
            GetTree().ChangeSceneToFile(RetryScene);
            CarryData.Instance.TotalLootValue = 0;
            CarryData.Instance.RemainingLootValue = 0;
        }
    }

    public void MainMenu()
    {
        GD.Print("Main Menu level: " + (MainMenuScene != null));
        if (MainMenuScene != null)
        {
            GetTree().ChangeSceneToFile(MainMenuScene);
            CarryData.Instance.TotalLootValue = 0;
            CarryData.Instance.RemainingLootValue = 0;
        }
    }

    private void clearLevel()
    {
        foreach(var child in GetTree().Root.GetChildren())
        {
            child.QueueFree();
        }
    }
}
