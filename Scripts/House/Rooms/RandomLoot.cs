using Godot;
using System;

public partial class RandomLoot : RoomRandom
{
    [Export(PropertyHint.Range, "0,1")] public float NoSpawnChance = 0.15f;
    [Export(PropertyHint.Range, "0,1")] public float KeyChance = 0.1f;
    [Export] public PackedScene Key;
    [Export] public PackedScene[] LootTable;

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void Randomize()
    {
		Randomizer = Randomizer ?? this;
        float nospawnPercent = (float)GD.RandRange(0f, 1f);
        if (nospawnPercent <= NoSpawnChance)
            QueueFree();
        else
        {
            int keyCount = CarryData.Instance.KeyCount;
            int maxKeyCount = CarryData.Instance.MaxKeyCount;
            float keyPercent = (float)GD.RandRange(0f, 1f);
            if (keyPercent <= KeyChance && keyCount < maxKeyCount)
            {
                summonLoot(Key);
                CarryData.Instance.KeyCount += 1;
            }
            else
            { 
                int table_size = LootTable.Length - 1;
                summonLoot(LootTable[GD.RandRange(0, table_size)]);
            }    
        }
    }

    private void summonLoot(PackedScene loot)
    {
        if (loot == null) return;

        Node3D lootNode = loot.Instantiate() as Node3D;
        GetTree().Root.AddChild(lootNode);

        lootNode.GlobalPosition = GlobalPosition;
        lootNode.RotationDegrees = Vector3.Up * (float)GD.RandRange(-180f, 180f);

        if (lootNode is Relic)
        {
            Relic relic = lootNode as Relic;
            CarryData.Instance.RemainingLootValue += relic.loot.GetValue();
        }
    }
}
