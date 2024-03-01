using Godot;
using System;
using System.Collections.Generic;

public partial class MusicGroup : AudioStreamPlayer
{
	public static MusicGroup Instance;

	[Export] public MusicTrack[] Tracks = new MusicTrack[0];

	private Dictionary<string, long> channels = new Dictionary<string, long>();
	private AudioStreamPlaybackPolyphonic playback;

	public override void _Ready()
	{
		Instance = this;

		if (Stream == null || !(Stream is AudioStreamPolyphonic))
		{
			Stream = new AudioStreamPolyphonic();
		}
		AudioStreamPlaybackPolyphonic phony = GetStreamPlayback() as AudioStreamPlaybackPolyphonic;
		foreach (MusicTrack track in Tracks)
		{
			track.playback = phony;
			track.SetupTrack();
		}
		playback = phony;
	}
}
