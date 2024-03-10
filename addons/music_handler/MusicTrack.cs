using Godot;
using Godot.Collections;
using System;

[GlobalClass]
[Tool]
public partial class MusicTrack : Resource
{

    [Export] public string Name
    {
        get { return name; }
        set { name = value; ResourceName = value; }
    }
    [Export] public MusicChannel[] Channels = new MusicChannel[0];
    //[Export] public MusicVenue Venue;

	private string name;
	private Dictionary<string, MusicChannel> channels = new Dictionary<string, MusicChannel>();
	public AudioStreamPlaybackPolyphonic playback {  get; set; }

    public float trackVolume = -80f;
    public float trackVolumeMin = 0f;
    public float trackVolumeMax = 1f;

	public void SetupTrack()
	{
		foreach (MusicChannel channel in Channels)
		{
			long id = playback.PlayStream(channel.Music, volumeDb: channel.VolumeDB);
			channel.StreamID = id;
			channels.Add(channel.Name, channel);
		}
    }

    public void StartTrack()
    {
        StopTrack();
        foreach (var channel in channels)
        {
            long id = playback.PlayStream(channel.Value.Music, volumeDb: channel.Value.VolumeDB);
            channel.Value.StreamID = id;
        }
    }

    public void StopTrack()
    {
        foreach (var channel in channels)
        {
            playback.StopStream(channel.Value.StreamID);
        }
    }

    public void SetVolume(float volume, bool use_curve = true)
    {
        foreach (var channel in channels)
        {
            SetChannelVolume(channel.Key, volume, use_curve);
        }
    }

    public void SetChannelVolume(string channel, float volume, bool use_curve = true)
    {
        MusicChannel chnl = channels[channel];
        playback.SetStreamVolume(chnl.StreamID, chnl.SetVolume(volume, use_curve));
        //GD.Print("Channel " + channel + " volume set to " + chnl.SetVolume(volume, use_curve));
    }

    public void SetActive(bool active)
    {
        foreach (var channel in channels)
        {
            SetChannelActive(channel.Key, active);
        }
    }

    public void SetChannelActive(string channel, bool active)
    {
        channels[channel].Active = active;
    }
}
