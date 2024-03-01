#if TOOLS
using Godot;
using System;

[Tool]
public partial class AnimationVariables : EditorPlugin
{
    private const string ScriptPath = "res://addons/anim_vars/AnimVariables.cs";
    private const string TexturePath = "res://addons/anim_vars/Icon.png";

    private const string VarScriptPath = "res://addons/anim_vars/AnimVariable.cs";
    private const string VarTexturePath = "res://addons/anim_vars/IconVar.png";

    private const string ScalScriptPath = "res://addons/anim_vars/AnimScalarExpression.cs";
    private const string ScalTexturePath = "res://addons/anim_vars/IconScal.png";

    private const string BranchScriptPath = "res://addons/anim_vars/AnimVariableBranch.cs";
    private const string BranchTexturePath = "res://addons/anim_vars/IconBranch.png";
    public override void _EnterTree()
	{
        // Initialization of the plugin goes here.
        var script = GD.Load<Script>(ScriptPath);
        var texture = GD.Load<Texture2D>(TexturePath);
        AddCustomType("AnimationVariables", "Node", script, texture);

        var scriptvar = GD.Load<Script>(VarScriptPath);
        var texturevar = GD.Load<Texture2D>(VarTexturePath);
        AddCustomType("AnimVariable", "Resource", scriptvar, texturevar);

        var scriptscal = GD.Load<Script>(ScalScriptPath);
        var texturescal = GD.Load<Texture2D>(ScalTexturePath);
        AddCustomType("AnimScalarExpression", "Resource", scriptscal, texturescal);

        var scriptbranch = GD.Load<Script>(BranchScriptPath);
        var texturebranch = GD.Load<Texture2D>(BranchTexturePath);
        AddCustomType("AnimVariableBranch", "Resource", scriptbranch, texturebranch);
    }

	public override void _ExitTree()
	{
        // Clean-up of the plugin goes here.
        RemoveCustomType("AnimationVariables");
        RemoveCustomType("AnimVariable");
        RemoveCustomType("AnimScalarExpression");
        RemoveCustomType("AnimVariableBranch");
    }
}
#endif
