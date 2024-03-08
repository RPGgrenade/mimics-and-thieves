using Godot;
using System;
using System.Linq;

public partial class Exit : Area3D
{
    [Export] public PackedScene WinningsScene;
    [Export] public string PlayerGroup;

    public void OnExiting(Node3D body)
    {
        if (WinningsScene != null && body.IsInGroup(PlayerGroup))
        {
            ThiefController player = body as ThiefController;
            int totalvalue = player.loot.TotalValue;

            GetTree().ChangeSceneToPacked(WinningsScene);
            CoinDropper dropper = GetTree().Root.GetChildren().OfType<CoinDropper>().First();
            
            dropper.TotalValue = totalvalue;
        }
    }
}
