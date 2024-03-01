#if TOOLS
using Godot;
using System;

[Tool]
public partial class InputBuffer : EditorPlugin
{
    private const string AutoLoadName = "InputActionBuffer";
    private const string ScenePath = "res://addons/input_buffer/global.tscn";
    private const string ScriptPath = "res://addons/input_buffer/InputActionBuffer.cs";
    private const string TexturePath = "res://addons/input_buffer/Icon.png";
    public override void _EnterTree()
	{
        // Initialization of the plugin goes here.
        var script = GD.Load<Script>(ScriptPath);
        var texture = GD.Load<Texture2D>(TexturePath);
        AddCustomType(AutoLoadName, "Node", script, texture);
        AddAutoloadSingleton(AutoLoadName, ScenePath);
    }

	public override void _ExitTree()
	{
        // Clean-up of the plugin goes here.
        RemoveCustomType(AutoLoadName);
        RemoveAutoloadSingleton(AutoLoadName);
    }
}
#endif
