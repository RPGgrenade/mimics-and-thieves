using Godot;
using System;

public partial class AreaExit : Area3D
{
    [Export] public Door ExitDoor;
    public void RoomExited(Node3D body)
    {
        if (body.IsInGroup("player"))
        {
            ExitDoor.Close();
        }
    }
}
