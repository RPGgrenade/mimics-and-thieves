using Godot;
using Godot.Collections;
using System;
using System.Linq;

public partial class Inventory : Node
{
	public partial class InventorySlot : Node {
		public Loot loot;
		public int count;

		public InventorySlot(Relic relic)
		{
			this.loot = relic.loot;
			count = 1;
		}
	}

	[Export] public bool Open = false;

    [Export] public Loot SelectedLoot;
    [Export] public int SelectedCount; 
    [Export] public int TotalValue;
    [Export] public Vector2 LossCount = new Vector2(2, 4);
    [Export] public Vector2 LossPower = new Vector2(4, 10);
    [Export] public Vector2 LossAngle = new Vector2(30f, 50f);

    [ExportCategory("Bag Visuals")]
    [ExportGroup("Bag")]
    [Export] public Node3D Hole;
    [Export] public Node3D Opening;
    [Export] public Node3D Bag;
    [ExportGroup("Size")]
    [Export] public float HoleLargeSize = 1.5f;
    [Export] public float OpeningLargeSize = 1.75f;
    [Export] public float BagLargeSize = 1.15f;
    [ExportGroup("Speed")]
    [Export] public float HoleSizeSpeed = 5f;
    [Export] public float OpeningSizeSpeed = 6f;
    [Export] public float BagSizeSpeed = 4f;

    private Vector3 holeScale;
    private Vector3 openScale;
    private Vector3 bagScale;

    public Dictionary<string, InventorySlot> _allLoot = new Dictionary<string, InventorySlot>();

    public override void _Ready()
    {
        holeScale = Hole.Scale;
        openScale = Opening.Scale;
        bagScale = Bag.Scale;
    }

    public override void _Process(double delta)
    {
        Hole.Scale = Hole.Scale.Lerp(holeScale * (Open ? HoleLargeSize : 1f), (float)delta * HoleSizeSpeed);
        Opening.Scale = Opening.Scale.Lerp(openScale * (Open ? OpeningLargeSize : 1f), (float)delta * OpeningSizeSpeed);
        Bag.Scale = Bag.Scale.Lerp(bagScale, (float)delta * BagSizeSpeed);
    }

    public void CycleRight()
    {
        if (_allLoot.Keys.Count > 0)
        {
            int index = _allLoot.Keys.ToList().IndexOf(SelectedLoot.name) + 1;
            if (index >= _allLoot.Keys.Count) index = 0;
            string key = _allLoot.Keys.ToList()[index];
            SelectedLoot = _allLoot[key].loot;
            SelectedCount = _allLoot[key].count;
        }
        GD.Print("Selected Loot is " + SelectedLoot.name);
    }

    public void CycleLeft()
    {
        if (_allLoot.Keys.Count > 0)
        {
            int index = _allLoot.Keys.ToList().IndexOf(SelectedLoot.name) - 1;
            if (index < 0) index = _allLoot.Keys.Count - 1;
            string key = _allLoot.Keys.ToList()[index];
            SelectedLoot = _allLoot[key].loot;
            SelectedCount = _allLoot[key].count;
        }
        GD.Print("Selected Loot is " + SelectedLoot.name);
    }

    public void AddLoot(Relic relic)
    {
        Loot loot = relic.loot;
        if (!_allLoot.ContainsKey(loot.name))
        {
            InventorySlot slot = new InventorySlot(relic);
            _allLoot.Add(loot.name, slot);
            if (_allLoot.Count == 1)
            {
                SelectedLoot = loot;
                SelectedCount = 1;
            }
        }
        else
        {
            _allLoot[loot.name].count += 1;
            if (loot.name == SelectedLoot.name) SelectedCount = _allLoot[loot.name].count;
        }
        Bag.Scale *= BagLargeSize;
        relic.QueueFree();
        countTotals();
    }

    public Node3D RemoveLoot(Loot selected = null)
    {
        Node3D relic = new Node3D();
        Loot loot = selected ?? SelectedLoot;
        GD.Print("Loot: "+_allLoot);
        if (_allLoot.ContainsKey(loot.name))
        {
            GD.Print("Taking " + loot.name + " out");
            // Count down loot count
            _allLoot[loot.name].count -= 1;

            string path = _allLoot[loot.name].loot.RelicPath;
            PackedScene scene = GD.Load<PackedScene>(path);
            relic = scene.Instantiate<Node3D>();
            GetTree().Root.AddChild(relic);

            if (_allLoot[loot.name].count <= 0)
            {
                // Remove loot from inventory
                _allLoot.Remove(loot.name);

                // Randomize key to select for the selected loot
                var keys = _allLoot.Keys.ToArray();
                int key_count = keys.Length;
                GD.Print("Keys: " + keys);
                if (key_count > 0)
                {
                    string rand_key = keys[GD.RandRange(0, key_count - 1)];
                    SelectedLoot = _allLoot[rand_key].loot;
                    SelectedCount = _allLoot[rand_key].count;
                }
                else { SelectedLoot = null; SelectedCount = 0; }
            }
            else
                SelectedCount = _allLoot[loot.name].count;

        }
        else {
            SelectedLoot = null; 
            SelectedCount = 0;
            countTotals();
            return null;
        }
        countTotals();
        return relic;
    }

    public void LoseLoot()
    {
        int lost_amount = (int)GD.RandRange(LossCount.X, LossCount.Y);
        for (int i = 0; i < lost_amount; i++)
        {
            var keys = _allLoot.Keys.ToList();
            int index = GD.RandRange(0, keys.Count-1);
            Relic item = RemoveLoot(_allLoot[keys[index]].loot) as Relic;
            if (item != null)
            {
                item.Position = new Vector3(0f, 1f, 0f);
                item.Rotation = new Vector3(0f, 0f, 0f);
                Vector3 forward = ((Owner as Node3D).Basis * Vector3.Back).Normalized();
                Vector3 right = ((Owner as Node3D).Basis * Vector3.Right).Normalized();
                float angle = Mathf.DegToRad((float)GD.RandRange(LossAngle.X, LossAngle.Y));

                Vector3 throwVector = forward.Rotated(right, angle).Normalized()
                    .Rotated(Vector3.Up, GD.RandRange(-180,180)).Normalized() 
                    * (float)GD.RandRange(LossPower.X, LossPower.Y);
                item.Used = true;
                item.UsedTimeout = 4f;
                item.IsUsed();
                item.SetVelocity(throwVector + (Owner as ThiefController).Velocity);
            }
        }
    }

    private void countTotals()
    {
        TotalValue = 0;
        foreach (var slot in _allLoot)
        {
            InventorySlot slotLoot = slot.Value;
            TotalValue += (slotLoot.loot.GetValue() * slotLoot.count);
        }
    }
}
