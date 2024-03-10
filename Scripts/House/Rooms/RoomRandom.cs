using Godot;
using System;

public partial class RoomRandom : Node3D
{
    [Export] public Node3D Randomizer;

    public virtual void Randomize() { return; }
}
