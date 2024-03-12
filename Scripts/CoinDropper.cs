using Godot;
using MEC;
using System;

public partial class CoinDropper : Node3D
{
    [Export] public CameraTransition camera;
    [Export] public CoinDropper NextDropper;
    [Export] public bool AutoDrops = true;
	[Export] public bool IsLootAcquired = true;
	[Export] public int TotalValue = 0;
	[ExportGroup("Coins")]
    [Export] public PackedScene BronzeCoin;
	[Export] public PackedScene SilverCoin;
	[Export] public PackedScene GoldCoin;
	[ExportGroup("Values")]
    [Export] public int BronzeValue = 10;
    [Export] public int SilverValue = 100;
    [Export] public int GoldValue = 1000;
	[ExportGroup("Speeds")]
	[Export] public float DropSpeed = 0.2f;
	[Export] public float CoinSpeed = 1.5f;

	private int totalValue = 0;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        if (IsLootAcquired)
            TotalValue = CarryData.Instance.TotalLootValue;
        else
            TotalValue = CarryData.Instance.RemainingLootValue;
        totalValue = TotalValue;
		if(TotalValue > 0 && AutoDrops)
            Timing.CallDelayed(DropSpeed, DropCoin);
    }

	private void MakeNextDrop()
    {
        if (NextDropper != null)
            Timing.CallDelayed(1.5f, NextDropper.SetDrop);
        if (camera != null)
            Timing.CallDelayed(0.7f, camera.SetTransition);
    }

	public void SetDrop()
    {
        if (TotalValue > 0)
            Timing.CallDelayed(DropSpeed, DropCoin);
    }

	private void DropCoin()
	{
		Relic coin = SpawnCoin();
		Vector3 velocity = new Vector3 (
			(float)GD.RandRange(-CoinSpeed, CoinSpeed), 
			0f, 
			(float)GD.RandRange(-CoinSpeed, CoinSpeed)
		);
		coin.SetVelocity(velocity);
		if (totalValue > 0)
			Timing.CallDelayed(DropSpeed, DropCoin);
		else
			MakeNextDrop();
    }

	private Relic SpawnCoin()
	{
		PackedScene chosenCoin = null;
		if (totalValue >= GoldValue)
		{
			chosenCoin = GoldCoin;
			totalValue -= GoldValue;
		}
		else if (totalValue >= SilverValue)
		{
			chosenCoin = SilverCoin;
			totalValue -= SilverValue;
		}
		else if (totalValue >= BronzeValue)
		{
			chosenCoin = BronzeCoin;
			totalValue -= BronzeValue;
		}

		Relic coin = chosenCoin.Instantiate() as Relic;
		this.AddChild(coin);
		coin.GlobalPosition = GlobalPosition;

		return coin;
	}
}
