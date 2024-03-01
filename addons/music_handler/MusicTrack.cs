using Godot;
using Godot.Collections;
using System;

[GlobalClass]
[Tool]
public partial class MusicTrack : Resource
{

	[Export] public MusicChannel[] Channels = new MusicChannel[0];

	private Dictionary<string, long> channels = new Dictionary<string, long>();
	public AudioStreamPlaybackPolyphonic playback {  get; set; }

	public void SetupTrack()
	{
		foreach (MusicChannel channel in Channels)
		{
			long id = playback.PlayStream(channel.Music, volumeDb: channel.VolumeDB);
			channels.Add(channel.Name, id);
		}
	}
}
