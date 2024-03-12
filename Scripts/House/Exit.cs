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

            GD.Print("Total Value: "+ totalvalue);

            GetTree().ChangeSceneToPacked(WinningsScene);
            CarryData.Instance.TotalLootValue = totalvalue;

            MusicHandler.Instance.SetGroupActive("Start Music", false, volSpeed: 1f);
            MusicHandler.Instance.SetGroupActive("Hall Music", false, volSpeed: 1f);
            MusicHandler.Instance.SetGroupActive("Room Music", false, volSpeed: 1f);
            MusicHandler.Instance.SetGroupActive("Caught Music", false, volSpeed: 1f);

            //CoinDropper dropper = GetTree().Root.GetChildren().OfType<CoinDropper>().First();
            //dropper.TotalValue = totalvalue;
        }
    }
}
