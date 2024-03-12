using Godot;
using System;

public partial class AreaExit : Area3D
{
    [Export] public Door ExitDoor;
    [Export] public string FightMusic = "Caught Music";
    public void RoomExited(Node3D body)
    {
        if (body.IsInGroup("player"))
        {
            ExitDoor.Close();
            MusicHandler.Instance.SetGroupActive(FightMusic, false, volSpeed: 1f);
        }
    }
}
