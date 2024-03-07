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
            Node3D target3D = target as Node3D;
            if (target3D.GlobalPosition.DistanceTo(GlobalPosition) <= Radius)
            {
                // Convert target to door script
                // unlock door target
                Door door = target3D as Door;
                door.Open();
                foundDoor = true;
                break;
            }
        }
        if (!foundDoor)
            key.Used = false;
    }
}
