using Godot;
using System;

[GlobalClass]
[Tool]
public partial class AnimScalarExpression : Resource
{
    [ExportCategory("Scalar Properties")]
    [Export] public string name
    {
        get => _name;
        set
        {
            _name = value;
            this.ResourceName = value;
        }
    }
    [Export(PropertyHint.PlaceholderText, "parameters/Root/anim/anim_speed/scale")]
    public string ScalarPath;
    [Export(PropertyHint.Expression)] public string Expression;

    private string _name;
}
