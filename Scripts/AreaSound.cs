using Godot;
using System;

public partial class AreaSound : Area3D
{
    [Export] public string AmbienceName = "";
    [Export] public string MusicName = "";

    public void StartSounds(Node3D body)
    {
        if (body.IsInGroup("player"))
        {
            if (AmbienceName != "")
                MusicHandler.Instance.SetGroupActive(AmbienceName, true);

            if (MusicName != "")
                MusicHandler.Instance.SetGroupActive(MusicName, true);
        }
    }

    public void StopSounds(Node3D body)
    {
        if (body.IsInGroup("player"))
        {
            if (AmbienceName != "")
                MusicHandler.Instance.SetGroupActive(AmbienceName, false);

            if (MusicName != "")
                MusicHandler.Instance.SetGroupActive(MusicName, false);
        }
    }
}
