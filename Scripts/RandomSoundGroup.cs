using Godot;
using System;

[GlobalClass]
public partial class RandomSoundGroup : Resource
{
    [Export] public string Name
    {
        get => name;
        set
        {
            name = value;
            this.ResourceName = value;
        }
    }
    [Export] public RandomSound[] Sounds;

    private string name;
}
