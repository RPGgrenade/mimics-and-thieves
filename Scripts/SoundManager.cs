using Godot;
using Godot.Collections;
using System;
using System.Linq;

[GlobalClass]
public partial class SoundManager : AudioStreamPlayer3D
{
    [Export] public RandomSoundGroup[] Sounds;
    private AudioStreamPlaybackPolyphonic playback;

    private Dictionary<string, RandomSoundGroup> _Sounds = new Dictionary<string, RandomSoundGroup>();

    public override void _Ready()
    {
        Autoplay = true;
        setPlayback();
        setSounds();
    }

    public void PlayRandom(string sound, bool useWeight = false, float volume = 0f, float minpitch = 1f, float maxpitch = 1f, float size = 10f, float offset = 0f)
    {
        UnitSize = size;
        if (playback == null)
            setPlayback();
        if(_Sounds.Count == 0)
            setSounds();
        if (_Sounds.ContainsKey(sound))
        {
            if (!useWeight)
            {
                int index = GD.RandRange(0, _Sounds[sound].Sounds.Length - 1);
                playback.PlayStream(_Sounds[sound].Sounds[index].Sound, offset, volume, (float)GD.RandRange(minpitch, maxpitch));
            }
        }
    }

    public void PlayOneShotRandom(AudioStream[] sound, float volume = 0f, float minpitch = 1f, float maxpitch = 1f, float size = 10f, float offset = 0f)
    {
        UnitSize = size;
        if (playback == null)
            setPlayback();
        if (sound != null && sound.Length > 0)
        {
            int index = GD.RandRange(0,sound.Length -1);
            playback.PlayStream(sound[index], offset, volume, (float)GD.RandRange(minpitch, maxpitch));
        }
    }

    public void PlayOneShot(AudioStream sound, float volume = 0f, float minpitch = 1f, float maxpitch = 1f, float size = 10f, float offset = 0f)
    {
        UnitSize = size;
        if (playback == null)
            setPlayback();
        if (sound != null)
            playback.PlayStream(sound, offset, volume, (float)GD.RandRange(minpitch, maxpitch));
    }

    private void setPlayback()
    {
        if (Stream == null || !(Stream is AudioStreamPolyphonic))
        {
            Stream = new AudioStreamPolyphonic();
        }
        AudioStreamPlaybackPolyphonic phony = GetStreamPlayback() as AudioStreamPlaybackPolyphonic;
        playback = phony;
    }

    private void setSounds()
    {
        Dictionary<string, RandomSoundGroup> soundGroups = new Dictionary<string, RandomSoundGroup>();
        foreach (var soundGroup in Sounds)
        {
            soundGroups[soundGroup.Name] = soundGroup;
        }
        _Sounds = soundGroups;
    }
}
