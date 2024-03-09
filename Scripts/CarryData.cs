using Godot;
using System;

public partial class CarryData : Node
{
	public static CarryData Instance;

	[ExportCategory("Loot")]
    [Export] public int TotalLootValue = 0;
    [Export] public int RemainingLootValue = 0;


    public override void _Ready()
	{
		Instance = this;
	}
}