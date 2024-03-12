using Godot;
using MEC;
using System;

public partial class RoomProcessing : Node3D
{
	[Export] public float CameraDistanceToProcess = 25f;
	[Export] public RandomRoom[] rooms;

	private Node3D camera;
	private float distanceFromCamera = float.MaxValue;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		camera = GetViewport().GetCamera3D();
		Timing.CallContinuously(double.MaxValue, CheckRooms);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public void CheckRooms()
	{
		if (camera != null && IsInstanceValid(camera))
		{
			foreach (var room in rooms)
			{
				if (room.listcount >= 5)
				{
					distanceFromCamera = room.GlobalPosition.DistanceTo(camera.GlobalPosition);
					if (distanceFromCamera > CameraDistanceToProcess || !room.ExitDoor.IsOpen)
					{
						room.ProcessMode = ProcessModeEnum.Disabled;
						room.Visible = false;
					}
					else if (distanceFromCamera <= CameraDistanceToProcess && room.ExitDoor.IsOpen)
					{
						room.ProcessMode = ProcessModeEnum.Inherit;
                        room.Visible = true;
                    }
				}
				//GD.Print("Room "+ room.Name + ": Active "+ room.ProcessMode);
			}
		}
	}
}
