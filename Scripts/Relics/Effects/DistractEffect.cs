using Godot;
using System;

public partial class DistractEffect : MagicEffect
{
    [Export] public PackedScene Balloon;
    [Export] public float Duration = 8f;
    [Export] public float OffsetForward = 2f;
    [Export] public float OffsetUp = -1f;

    public override void Activate()
    {
        GD.Print("Distract activated");
        Relic balloon = Balloon.Instantiate() as Relic;
        if (balloon != null)
        {
            Node3D owner = GetTree().GetFirstNodeInGroup("player") as Node3D;
            balloon.UsedTimeout = Duration;
            //balloon.IsUsed();

            GetTree().Root.AddChild(balloon);
            balloon.GlobalRotation = owner.GlobalRotation;
            balloon.GlobalPosition = owner.GlobalPosition
                + ((owner.Basis * Vector3.Forward).Normalized() * OffsetForward)
                + ((owner.Basis * Vector3.Up).Normalized() * OffsetUp)
            ;
        }
    }
}
