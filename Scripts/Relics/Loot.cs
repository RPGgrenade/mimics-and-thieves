using Godot;
using System;

[GlobalClass]
public partial class Loot : Resource
{
    public enum LootCategory { 
        Jewel,
        Relic,
        Rare
    }

    [Export] public string name;
    [Export(PropertyHint.MultilineText)] public string description;
    [Export] public Texture2D UI;
    [Export(PropertyHint.File)] public string RelicPath;
    [ExportCategory("Value")]
    [Export] public int Money = 100;
    [Export] public LootCategory Category = LootCategory.Jewel;
    [ExportCategory("Magic")]
    [Export] public Magic magic;

    public int GetValue()
    {
        return Money;
    }
}
