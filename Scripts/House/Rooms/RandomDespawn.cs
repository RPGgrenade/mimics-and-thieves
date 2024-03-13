using Godot;
using System;

public partial class RandomDespawn : RoomRandom
{
    [Export(PropertyHint.Range, "0,1")] public float DespawnChance = 0.1f;
    [Export] public float MinDistanceFromDoor = 12f;

    public override void Randomize()
    {
        Randomizer = Randomizer ?? this;
        float despawnPercent = (float)GD.RandRange(0f, 1f);
        if (despawnPercent <= DespawnChance)
            QueueFree();
        var doors = GetTree().GetNodesInGroup("door");
        foreach (var door in doors)
        {
            Node3D door3D = door as Node3D;
            if (door3D.GlobalPosition.DistanceTo(GlobalPosition) < MinDistanceFromDoor)
            {
                QueueFree();
                break;
            }
        }
    }
}
