using Godot;
using System;

[GlobalClass]
[Tool]
public partial class MusicChannel : Resource
{
	[Export]
	public string Name
	{
		get { return name; }
		set { name = value; ResourceName = value; }
	}
	[Export] public AudioStream Music;
	[ExportGroup("Volume")]
    [Export] public float VolumeDB
    {
        get { return vol_db; }
        set { vol_db = Mathf.Clamp(value, -80f, 24f); }
    }
	[Export] public Curve VolumeCurve { 
		get
		{
			if (vol_curve.PointCount == 0)
            {
				vol_curve.MinValue = -80f;
				vol_curve.MaxValue = 24f;
                vol_curve.AddPoint(Vector2.One * vol_curve.MinValue, rightTangent: 104f, leftMode: Curve.TangentMode.Linear);
                vol_curve.AddPoint(Vector2.One * vol_curve.MaxValue, leftTangent: 104f, leftMode: Curve.TangentMode.Linear);
            }
			return vol_curve;
		}
		set { vol_curve = value; }
	}
    [Export] public Vector2 VolumeRange { 
		get { return vol_range; } 
		set
        {
            vol_curve.MinValue = vol_range.X;
            vol_curve.MaxValue = vol_range.Y;
            VolumeDB = Mathf.Clamp(VolumeDB, vol_range.X, vol_range.Y);
			vol_range = value; 
		}
	}

	private string name;
	private float vol_db = -80f;
	private Vector2 vol_range = new Vector2(-80f, 24f);
	private Curve vol_curve = new Curve();
}
