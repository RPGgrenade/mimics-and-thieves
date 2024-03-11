using Godot;
using System;

public partial class RandomLighter : RoomRandom
{
    [Export] public OmniLight3D[] Lights;
    [Export] public float LightEnergyMin = 0.2f;
    [Export] public float LightEnergyMax = 0.5f;
    [Export] public Gradient Colors;

    public override void Randomize()
    {
        Randomizer = Randomizer ?? this;
        if (Lights != null && Lights.Length > 0)
        {
            Color color = Colors.GetColor(GD.RandRange(0, Colors.GetPointCount() - 1));
            float colorEnergy = (float)GD.RandRange(LightEnergyMin, LightEnergyMax);

            foreach (var light in Lights)
            {
                light.LightEnergy = colorEnergy;
                light.LightColor = color;
            }

            GD.Print("Color: " + color + ", Energy: " + colorEnergy);
        }
    }
}
