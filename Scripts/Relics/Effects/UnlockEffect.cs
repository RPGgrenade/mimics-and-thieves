using Godot;
using System;

public partial class UnlockEffect : MagicEffect
{
    [Export] public Relic key;
    [Export] public string EffectGroup = "door";
    [Export] public float Radius = 5f;

    public override void Activate()
    {
        GD.Print("Unlock activated");
        var targets = GetTree().GetNodesInGroup(EffectGroup);
        bool foundDoor = false;
        foreach (Node target in targets)
        {
            Door door = target as Door;
            if (door.GlobalPosition.DistanceTo(GlobalPosition) <= Radius && !door.IsOpen)
            {
                // Convert target to door script
                // unlock door target
                door.Open();
                foundDoor = true;
                break;
            }
        }
        if (!foundDoor)
            key.Used = false;
    }
}
