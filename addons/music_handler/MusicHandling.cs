#if TOOLS
using Godot;
using System;

[Tool]
public partial class MusicHandling : EditorPlugin
{
    private const string AutoLoadName = "MusicHandler";
    private const string ScenePath = "res://addons/music_handler/music.tscn";

    private const string GroupScriptPath = "res://addons/music_handler/MusicGroup.cs";
    private const string GroupTexturePath = "res://addons/music_handler/Group.png";

    private const string TrackScriptPath = "res://addons/music_handler/MusicTrack.cs";
    private const string TrackTexturePath = "res://addons/music_handler/Track.png";

    private const string ChannelScriptPath = "res://addons/music_handler/MusicChannel.cs";
    private const string ChannelTexturePath = "res://addons/music_handler/Channel.png";

    public override void _EnterTree()
    {
        // Initialization of the plugin goes here.

        var groupscript = GD.Load<Script>(GroupScriptPath);
        var grouptexture = GD.Load<Texture2D>(GroupTexturePath);
        AddCustomType("MusicGroup", "AudioStreamPlayer", groupscript, grouptexture);

        var trackscript = GD.Load<Script>(TrackScriptPath);
        var tracktexture = GD.Load<Texture2D>(TrackTexturePath);
        AddCustomType("MusicTrack", "Resource", trackscript, tracktexture);

        var channelscript = GD.Load<Script>(ChannelScriptPath);
        var channeltexture = GD.Load<Texture2D>(ChannelTexturePath);
        AddCustomType("MusicChannel", "Resource", channelscript, channeltexture);

        AddAutoloadSingleton(AutoLoadName, ScenePath);
    }

    public override void _ExitTree()
    {
        // Clean-up of the plugin goes here.
        RemoveCustomType("MusicGroup");
        RemoveCustomType("MusicTrack");
        RemoveCustomType("MusicChannel");

        RemoveAutoloadSingleton(AutoLoadName);
    }
}
#endif
