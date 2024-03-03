using Godot;
using System;

[GlobalClass]
public partial class RevealEffect : MagicEffect
{
    [Export] public string[] EffectGroups;
    [Export] public float Radius = 10f;
    [Export] public float Duration = 5f;

    public override void Activate()
    {
        GD.Print("Reveal activated");
        foreach (string group in EffectGroups)
        {
            var targets = GetTree().GetNodesInGroup(group);
            foreach (Node target in targets)
            {
                Node3D target3D = target as Node3D;
                if (GlobalPosition.DistanceTo(target3D.GlobalPosition) <= Radius)
                {
                    Node3D glow = target3D.FindChild("Glow") as Node3D;
                    if (glow != null)
                    {
                        Glow glowScript = glow as Glow;
                        glowScript.SetGlowTime(Duration);
                    }
                }
            }
        }
    }
}
