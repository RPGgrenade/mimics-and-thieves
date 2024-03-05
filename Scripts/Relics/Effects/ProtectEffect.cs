using Godot;
using System;

public partial class ProtectEffect : MagicEffect
{
    [Export] public PackedScene Barrier;
    [Export] public string EffectGroup;
    [Export] public string ExcludeEffectGroup;
    [Export] public float Duration = 8f;

    public override void Activate()
    {
        GD.Print("Protect activated");
        var targets = GetTree().GetNodesInGroup(EffectGroup);
        foreach (Node target in targets) // should only ever be 1 player
        {
            if (!target.IsInGroup(ExcludeEffectGroup))
            {
                Node3D target3D = target as Node3D;
                Node3D barrier = Barrier.Instantiate() as Node3D;
                if (barrier != null)
                {
                    Barrier barrierScript = barrier as Barrier;
                    barrierScript.Time = Duration;
                    target3D.AddChild(barrierScript);
                    barrierScript.controller = target3D as ThiefController;
                    barrierScript.RotationDegrees = Vector3.Zero;
                    barrierScript.Position = new Vector3(0f, 1f, 0f);
                }
            }
        }
    }
}
