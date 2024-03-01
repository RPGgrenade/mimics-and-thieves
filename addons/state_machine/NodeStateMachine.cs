#if TOOLS
using Godot;
using System;

[Tool]
public partial class NodeStateMachine : EditorPlugin
{
    private const string MachineScriptPath = "res://addons/state_machine/StateMachine.cs";
    private const string MachineTexturePath = "res://addons/state_machine/IconM.png";

    private const string StateScriptPath = "res://addons/state_machine/State.cs";
    private const string StateTexturePath = "res://addons/state_machine/IconS.png";
    public override void _EnterTree()
	{
        // Initialization of the plugin goes here.
        var machinescript = GD.Load<Script>(MachineScriptPath);
        var machinetexture = GD.Load<Texture2D>(MachineTexturePath);
        AddCustomType("StateMachine", "Node", machinescript, machinetexture);

        var statescript = GD.Load<Script>(StateScriptPath);
        var statetexture = GD.Load<Texture2D>(StateTexturePath);
        AddCustomType("State", "Node", statescript, statetexture);
    }

	public override void _ExitTree()
	{
        // Clean-up of the plugin goes here.
        RemoveCustomType("StateMachine");
        RemoveCustomType("State");
    }
}
#endif
