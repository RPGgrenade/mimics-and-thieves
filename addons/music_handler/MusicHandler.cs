using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class MusicHandler : Node
{
    public static MusicHandler Instance;

	[Export] public MusicGroup[] MusicGroups;

	private Dictionary<string,  MusicGroup> musicGroups = new Dictionary<string, MusicGroup>();

    private string lastMusicGroup = "";

    public override void _Ready()
	{
		Instance = this;
		if(MusicGroups == null || MusicGroups.Length == 0)
			MusicGroups = GetChildren(true).OfType<MusicGroup>().ToArray();
		foreach (MusicGroup group in MusicGroups)
		{
			musicGroups[group.Name] = group;
			//GD.Print("Music Groups: " + group.Name);
        }
    }

    public void SetGroupActive(string groupName, bool active, float volSpeed = 1f, bool force = false)
    {
        MusicGroup group = musicGroups[groupName];
        if (active)
        {
            //GD.Print("Music Handler: "+ groupName + " Group is beng set to active" );
            if (force) group.ForceStart();
            else group.SlowStart(volSpeed);
        }
        else
        {
            //GD.Print("Music Handler: " + groupName + " Group is beng set to inactive");
            if (force) group.ForceStop();
            else group.SlowStop(volSpeed);
        }
    }

    public void SetOnlyGroupActive(string groupName, float volSpeed = 1f, bool force = false)
    {
        foreach (var groupkvp in musicGroups)
        {
            if (force) groupkvp.Value.ForceStop();
            else groupkvp.Value.SlowStop(volSpeed);
        }
        MusicGroup group = musicGroups[groupName];
        //GD.Print("Music Handler: "+ groupName + " Group is beng set to active" );
        if (force) group.ForceStart();
        else group.SlowStart(volSpeed);
    }

    public void SetPriorityGroupActive(string groupName, bool active, float volSpeed = 1f, bool force = false)
    {
        if (lastMusicGroup == "") lastMusicGroup = groupName;
        else if (lastMusicGroup != "" && active) return;
        foreach (var groupkvp in musicGroups)
        {
            if (force) groupkvp.Value.ForceStop();
            else groupkvp.Value.SlowStop(volSpeed);
        }

        MusicGroup group = musicGroups[groupName];
        if (active)
        {
            //GD.Print("Music Handler: "+ groupName + " Group is beng set to active" );
            if (force) group.ForceStart();
            else group.SlowStart(volSpeed);
        }
        else
        {
            //GD.Print("Music Handler: " + groupName + " Group is beng set to inactive");
            if (force) group.ForceStop();
            else group.SlowStop(volSpeed);

            MusicGroup lastgroup = musicGroups[lastMusicGroup];
            lastMusicGroup = "";
            if (force) lastgroup.ForceStop();
            else lastgroup.SlowStop(volSpeed);
        }
    }
}
