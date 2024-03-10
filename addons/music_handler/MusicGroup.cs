using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Channels;

public partial class MusicGroup : AudioStreamPlayer
{
    [Export] public bool Active = false;
    [Export] public MusicTrack[] Tracks = new MusicTrack[0];
    //[Export] public MusicVenue Venue;

	private Dictionary<string, MusicTrack> tracks = new Dictionary<string, MusicTrack>();
	private AudioStreamPlaybackPolyphonic playback;

    private float volSpeed = 1f;

    public override void _Ready()
    {
        if (Stream == null || !(Stream is AudioStreamPolyphonic))
        {
            Stream = new AudioStreamPolyphonic();
        }
        AudioStreamPlaybackPolyphonic phony = GetStreamPlayback() as AudioStreamPlaybackPolyphonic;
        foreach (MusicTrack track in Tracks)
        {
            track.playback = phony;
            track.SetupTrack();
            tracks.Add(track.Name, track);
        }
        playback = phony;
    }

    public override void _Process(double delta)
    {
        if (Active)
        {
            foreach (MusicTrack track in Tracks)
            {
                track.trackVolume += (float)delta * volSpeed; 
                track.trackVolume = Mathf.Clamp(track.trackVolume, track.trackVolumeMin, track.trackVolumeMax);
                track.SetVolume(track.trackVolume);
                //GD.Print("Track " + track.Name + " volume set to " + track.trackVolume);
            }
        }
        else
        {
            foreach (MusicTrack track in Tracks)
            {
                track.trackVolume -= (float)delta * volSpeed;
                track.trackVolume = Mathf.Clamp(track.trackVolume, track.trackVolumeMin, track.trackVolumeMax);
                track.SetVolume(track.trackVolume);
                if(track.trackVolume == track.trackVolumeMin)
                    track.StopTrack();
                //GD.Print("Track " + track.Name + " volume set to " + track.trackVolume);
            }
        }
    }

    public void ForceStart()
    {
        Active = true;
        foreach (MusicTrack track in Tracks)
        {
            track.SetVolume(1f, true);
            track.StartTrack();
        }
    }

    public void SlowStart(float speed = 104f)
    {
        Active = true;
        volSpeed = speed;
        foreach (MusicTrack track in Tracks)
        {
            track.StartTrack();
        }
    }

    public void ForceStop()
    {
        Active = false;
        foreach (MusicTrack track in Tracks)
        {
            track.SetVolume(0f, true);
            track.StopTrack();
        }
    }

    public void SlowStop(float speed = 104f)
    {
        Active = false;
        volSpeed = speed;
    }

    public Variant CalculateExpression(string expression)
    {
        Expression exp = new Expression();
        Error err = exp.Parse(expression);
        if (err == Error.Ok)
        {
            Variant result = exp.Execute(baseInstance: this);
            if (!exp.HasExecuteFailed())
                return result;
            else
            {
                GD.PrintErr("Expression '" + expression + "' not executing a valid result.");
                return false;
            }
        }
        else
        {
            GD.PrintErr("Expression error: " + err);
            return false;
        }
    }
}
