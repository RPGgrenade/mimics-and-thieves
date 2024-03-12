using Godot;
using System;

public partial class RandomMimic : RoomRandom
{
    [Export(PropertyHint.Range, "0,1")] public float NoSpawnChance = 0.08f;
    [Export(PropertyHint.Range, "0,1")] public float FurnitureChance = 0.6f;
    [Export] public PackedScene[] MimicTable;

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void Randomize()
    {
        Randomizer = Randomizer ?? this;
        float nospawnPercent = (float)GD.RandRange(0f, 1f);
        if (nospawnPercent <= NoSpawnChance)
            QueueFree();
        else
        {
            int table_size = MimicTable.Length - 1;
            summonMimic(MimicTable[GD.RandRange(0, table_size)]);
        }
    }

    private void summonMimic(PackedScene mimic)
    {
        if (mimic == null) return;

        Node3D mimicNode = mimic.Instantiate() as Node3D;
        GetTree().Root.AddChild(mimicNode);

        mimicNode.GlobalPosition = GlobalPosition;
        mimicNode.RotationDegrees = Vector3.Up * (float)GD.RandRange(-180f, 180f);

        if (mimicNode is Mimic)
        {
            Mimic mimick = mimicNode as Mimic;
            float furnitureChance = (float)GD.RandRange(0f,1f);
            mimick.IsMimic = furnitureChance > FurnitureChance;
        }
    }
}
