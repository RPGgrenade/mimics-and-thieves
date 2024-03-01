using Godot;
using System;

public partial class CameraController : SpringArm3D
{
    [ExportCategory("Scripts")]
    [Export]
    public ThiefInputs inputs;

    [ExportCategory("Camera Options")]
    [Export]
    public Camera3D camera;
    [Export]
    public float mouse_sensitivity = 0.05f;
    [Export]
    public float stick_sensitivity = 1.25f;
    [Export]
    public Vector2 VerticalAngleLimit = new Vector2(-90f, 30f);


    private Vector3 rotation_degrees = Vector3.Zero;
    [Export]
    private SphereShape3D sphere;


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        TopLevel = true;
    }

    public override void _Process(double delta)
    {
        Vector2 mouse_input = inputs.MouseInput;
        Vector2 stick_input = inputs.RightStick;
        float deltaTime = (float)delta;
        rotation_degrees = RotationDegrees;

        rotation_degrees = processRotationInput(mouse_input, stick_input, rotation_degrees, deltaTime);

        RotationDegrees = rotation_degrees;
    }

    public void EvadeCollision(Vector3 rotation_degrees, float deltaTime)
    {
        var direct_state = GetWorld3D().DirectSpaceState;
        var result = direct_state.IntersectRay(
            PhysicsRayQueryParameters3D.Create(camera.Position, camera.Position + (camera.Basis.Z * 100f))
        );
        if (result.Count > 0)
        {
            GD.Print("Hit info is " + result);
        }
    }

    private Vector3 processRotationInput(Vector2 cam_input, Vector2 stick_input, Vector3 rotation_degrees, float deltaTime)
    {
        rotation_degrees.X = Mathf.Lerp(rotation_degrees.X, rotation_degrees.X - (cam_input.Y * mouse_sensitivity + stick_input.Y * stick_sensitivity), deltaTime);
        rotation_degrees.X = Mathf.Clamp(rotation_degrees.X, VerticalAngleLimit.X, VerticalAngleLimit.Y);
        rotation_degrees.Y = Mathf.Lerp(rotation_degrees.Y, rotation_degrees.Y - (cam_input.X * mouse_sensitivity + stick_input.X * stick_sensitivity), deltaTime);
        rotation_degrees.Y = Mathf.Wrap(rotation_degrees.Y, 0f, 360f);
        return rotation_degrees;
    }
}
