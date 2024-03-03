using Godot;
using System;

[GlobalClass]
public partial class Magic : Resource
{
    public enum MagicType
    {
        Magic, Effect
    }

    [Export] public string name;
    [Export] public MagicType type = MagicType.Magic;
    [Export(PropertyHint.MultilineText)] public string Description;
}
