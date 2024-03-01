using Godot;
using Godot.Collections;
using System;

[GlobalClass]
[Tool]
public partial class AnimVariableBranch : Resource
{
    [Export]
    public string Name
    {
        get => name;
        set
        {
            name = value;
            this.ResourceName = value;
        }
    }
    [Export] public AnimVariable[] Variables = new AnimVariable[0];
    [Export] public bool Register = true;

    private string name;
}
