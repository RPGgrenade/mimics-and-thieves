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
	[Export] public bool Active;
	[Export] public AudioStream Music;
	//[Export] public MusicVenue Venue;
	[ExportGroup("Volume")]
    [Export] public float VolumeDB
    {
        get { return vol_db; }
        set { vol_db = Mathf.Clamp(value, vol_range.X, vol_range.Y); }
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
            vol_curve.MinValue = value.X;
            vol_curve.MaxValue = value.Y;
            VolumeDB = Mathf.Clamp(VolumeDB, value.X, value.Y);
			vol_range = value; 
		}
	}

	private string name;
	private float vol_db = -80f;
	private Vector2 vol_range = new Vector2(-80f, 24f);
	private Curve vol_curve = new Curve();

	public long StreamID { get { return id; } set { id = value; } }
	private long id;

	public float SetVolume(float volume, bool use_curve = true)
	{
		if(use_curve)
			VolumeDB = VolumeCurve.Sample(volume);
		else
			// Incorrect, needs to calculate linearly between min and max using 0 to 1 as a range
			VolumeDB = volume;
		return VolumeDB;
	}
}
