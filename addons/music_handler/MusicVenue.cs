using Godot;
using System;


[GlobalClass]
[Tool]
public partial class MusicVenue : Resource
{
    public enum VenueType
    {
        Location, Variable, Expression
    }

    [Export]
    public VenueType Venue
    {
        get => _venue;
        set
        {
            _venue = value;
            NotifyPropertyListChanged();
        }
    }



    public string Expression;
    public Vector3 Location = Vector3.Zero;
    public float Radius = 1f;

    private VenueType _venue = VenueType.Expression;


    public override Godot.Collections.Array<Godot.Collections.Dictionary> _GetPropertyList()
    {
        var properties = new Godot.Collections.Array<Godot.Collections.Dictionary>();
        switch (Venue)
        {
            case VenueType.Expression:
                {
                    properties.Add(createProperty("Expression", Variant.Type.String, PropertyUsageFlags.Default, PropertyHint.Expression, "Venue Expression"));
                    return properties;
                }
            case VenueType.Location:
                {
                    properties.Add(createProperty("Location", Variant.Type.Vector3, PropertyUsageFlags.Default, PropertyHint.None, "Venue Location"));
                    properties.Add(createProperty("Radius", Variant.Type.Float, PropertyUsageFlags.Default, PropertyHint.None, "Venue Radius"));
                    return properties;
                }
            default: return null;
        }
    }

    private Godot.Collections.Dictionary createProperty(string varName, Variant.Type type, PropertyUsageFlags usage, PropertyHint hint_type, string ui_hint)
    {
        var property = new Godot.Collections.Dictionary()
        {
            { "name", varName },
            { "type", (int)type },
            { "usage", (int)usage }, // See above assignment.
            { "hint", (int)hint_type },
            { "hint_string", ui_hint }
        };
        return property;
    }
}
