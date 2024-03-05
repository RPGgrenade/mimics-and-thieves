using Godot;
using System;

public partial class UnlockEffect : MagicEffect
{
    [Export] public string EffectGroup = "door";
    [Export] public float Radius = 5f;

    public override void Activate()
    {
        GD.Print("Unlock activated");
        var targets = GetTree().GetNodesInGroup(EffectGroup);
        foreach (Node target in targets)
        {
            Node3D target3D = target as Node3D;
            if (target3D.GlobalPosition.DistanceTo(GlobalPosition) <= Radius)
            {
                // Convert target to door script
                // unlock door target
                break;
            }
        }
    }
}
