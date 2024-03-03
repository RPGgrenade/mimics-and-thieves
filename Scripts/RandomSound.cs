using Godot;
using System;

[GlobalClass]
public partial class RandomSound : Resource
{
    [Export] public float Weight = 1f;
    [Export] public AudioStream Sound;
}
