using Godot;
using System;

public partial class RandomDespawn : RoomRandom
{
    [Export(PropertyHint.Range, "0,1")] public float DespawnChance = 0.1f;

    public override void Randomize()
    {
        Randomizer = Randomizer ?? this;
        float despawnPercent = (float)GD.RandRange(0f, 1f);
        if (despawnPercent <= DespawnChance)
            QueueFree();
    }
}
