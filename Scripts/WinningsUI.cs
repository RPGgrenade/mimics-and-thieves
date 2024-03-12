using Godot;
using System;

public partial class WinningsUI : CanvasLayer
{
	[Export] public Control Focus;

    [Export] public PackedScene RetryScene;
    [Export] public PackedScene MainMenuScene;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		Focus.GrabFocus();
    }

    public void Retry()
    {
        if (RetryScene != null)
        {
            GetTree().ChangeSceneToPacked(RetryScene);
            CarryData.Instance.TotalLootValue = 0;
            CarryData.Instance.RemainingLootValue = 0;
        }
    }

    public void MainMenu()
    {
        if (MainMenuScene != null)
        {
            GetTree().ChangeSceneToPacked(MainMenuScene);
            CarryData.Instance.TotalLootValue = 0;
            CarryData.Instance.RemainingLootValue = 0;
        }
    }
}
