using Godot;
using System;

[GlobalClass]
[Tool]
public partial class AnimVariable : Resource
{
    public enum VarType
    {
        Bool, Float, Int, String
    }

    [ExportCategory("Variable Properties")]
    [Export] public VarType variableType { 
        get => _variableType; 
        set
        {
            _variableType = value;
            NotifyPropertyListChanged();
        }
    }
    [Export] public string variableName
    {
        get => _variableName;
        set
        {
            _variableName = value;
            this.ResourceName = value;
        }
    }
    [Export] public bool Constant = false;

    public bool boolValue;
    public float floatValue;
    public int intValue;
    public string stringValue;

    private VarType _variableType;
    private string _variableName;

    public override Godot.Collections.Array<Godot.Collections.Dictionary> _GetPropertyList()
    {
        switch (variableType)
        {
            case VarType.Bool: return createProperty("boolValue", Variant.Type.Bool, PropertyUsageFlags.Default, "Boolean value");
            case VarType.Float: return createProperty("floatValue", Variant.Type.Float, PropertyUsageFlags.Default, "Float value");
            case VarType.Int: return createProperty("intValue", Variant.Type.Int, PropertyUsageFlags.Default, "Integer value");
            case VarType.String: return createProperty("stringValue", Variant.Type.String, PropertyUsageFlags.Default, "String value");
            default: return null;
        }
    }

    private Godot.Collections.Array<Godot.Collections.Dictionary> createProperty(string varName, Variant.Type type, PropertyUsageFlags usage, string ui_hint)
    {
        var properties = new Godot.Collections.Array<Godot.Collections.Dictionary>();
        properties.Add(new Godot.Collections.Dictionary()
        {
            { "name", varName },
            { "type", (int)type },
            { "usage", (int)usage }, // See above assignment.
            { "hint", (int)PropertyHint.None },
            { "hint_string", ui_hint }
        });
        return properties;
    }

    public Variant Value
    {
        get
        {
            switch (variableType)
            {
                case VarType.Int: return intValue;
                case VarType.Float: return floatValue;
                case VarType.Bool: return boolValue;
                case VarType.String: return stringValue;
                default:
                    GD.PrintErr("Variable " + variableName + " has no type. Set to type to get its value.");
                    return false;
            }
        }   // get method
        set
        {
            if (!Constant)
            {
                switch (variableType)
                {
                    case VarType.Int: intValue = (int)value; break;
                    case VarType.Float: floatValue = (float)value; break;
                    case VarType.Bool: boolValue = (bool)value; break;
                    case VarType.String: stringValue = (string)value; break;
                }
            }
            else
                GD.PrintErr("Variable " + variableName + " is constant. Set to not consant to set its value.");
        }  // set method
    }
}
