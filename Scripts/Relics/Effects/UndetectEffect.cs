using Godot;
using System;

public partial class UndetectEffect : MagicEffect
{
    [Export] public string EffectGroup;
    [Export] public string ExcludeEffectGroup;
    [Export] public float Duration = 5f;

    public override void Activate()
    {
        GD.Print("Undetect activated");
        var targets = GetTree().GetNodesInGroup(EffectGroup);
        foreach (Node target in targets)
        {
            if (!target.IsInGroup(ExcludeEffectGroup))
            {
                ThiefController target3D = target as ThiefController;
                target3D.SetUndetectable(Duration);
            }
        }
    }
}
