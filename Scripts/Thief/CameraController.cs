using Godot;
using Microsoft.VisualBasic.FileIO;
using System;

public partial class CameraController : SpringArm3D
{
    [ExportCategory("Scripts")]
    [Export] public ThiefInputs inputs;

    [ExportCategory("Camera Options")]
    [Export] public Camera3D camera;

    [ExportGroup("Sensitivity")]
    [Export] public float mouse_sensitivity = 0.05f;
    [Export] public float stick_sensitivity = 1.25f;
    [Export] public Vector2 VerticalAngleLimit = new Vector2(-90f, 30f);

    [ExportGroup("Speeds")]
    [Export] public float CameraAcceleration = 2f;
    [Export] public float CameraSpeed = 1f;

    [ExportGroup("Evasion")]
    [ExportSubgroup("Surrounding")]
    [Export] public ShapeCast3D EvasionRay;
    [Export] public float ControlMultiplier = 0.5f;
    [Export] public float SurroundingDistance = 5f;
    [Export] public float SurroundingSpeed = 40f;
    [ExportSubgroup("Zooming")]
    [Export] public ShapeCast3D InterceptionRay;
    [Export] public float ZoomDistance = 15f;
    [Export] public float ZoomingInSpeed = 40f;
    [Export] public float ZoomingOutSpeed = 20f;
    //[ExportSubgroup("Sloping")]
    //[Export] public float SlopeDistance = 3f;
    //[Export] public float SlopeSpeed = 30f;

    [ExportGroup("Guidance")]
    [ExportSubgroup("Return")]
    [Export] public float ReturnTime = 4f;
    [Export] public float ReturnThreshold = 5f;
    [Export] public float ReturnSpeed = 30f;
    [Export] public float ReturnYAngle = 180f;
    [Export] public float ReturnXAngle = -10f;
    [ExportSubgroup("Offset")]
    [Export] public float OffsetThreshold = 2f;
    [Export] public float OffsetAmount = 0.75f;
    [Export] public float OffsetSpeed = 2f;

    private float cameraTargetSpeed = 0f;
    private float cameraSpeed = 0f;
    private Vector3 rotation_degrees = Vector3.Zero;

    private float returnTime = 0f;
    private float startDistance = 0f;
    private float currentDistance = 0f;
    private float targetDistance = 0f;
    private Node3D target;


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        target = Owner as Node3D;
        TopLevel = true;
        cameraTargetSpeed = CameraSpeed;
        returnTime = ReturnTime;
        startDistance = SpringLength;
        currentDistance = SpringLength;
        targetDistance = SpringLength;
    }

    public override void _PhysicsProcess(double delta)
    {
        Vector2 mouse_input = inputs.MouseInput;
        Vector2 stick_input = inputs.RightStick;
        float deltaTime = (float)delta;
        rotation_degrees = RotationDegrees;


        // Booleans for determing acceleration times
        bool camera_input = (mouse_input.Length() > 0.1f || stick_input.Length() > 0.1f);
        float rot_difference = (180.0f - Mathf.RadToDeg((rotation_degrees.Y - target.GlobalRotationDegrees.Y - ReturnYAngle))) / 180.0f;
        bool is_returning = (returnTime <= 0f && Mathf.Abs(rot_difference) >= ReturnThreshold);
        bool is_evading = (EvasionRay.IsColliding() || InterceptionRay.IsColliding());
        bool is_accelerating = (camera_input || is_returning || is_evading);

        cameraSpeed = Mathf.Lerp(cameraSpeed, is_accelerating ? cameraTargetSpeed : 0f, deltaTime);
        rotation_degrees = processRotationInput(mouse_input, stick_input, rotation_degrees, deltaTime);
        updateOffset(rot_difference, (float)deltaTime);

        if (!camera_input)
        {
            evadeSurroundings(camera_input, (float)delta);
            cameraTargetSpeed = ReturnSpeed;
            returnTime -= deltaTime;
            if (returnTime <= 0f)
                rotation_degrees = returnRotation(rotation_degrees, deltaTime);
        }
        else
        {
            cameraTargetSpeed = CameraSpeed;
            returnTime = ReturnTime;
        }

        updateDistance((float)delta);
        
        RotationDegrees = rotation_degrees;
    }

    private void updateDistance(float delta)
    {
        float obstacleDistance = targetDistance;
        if (InterceptionRay.IsColliding())
        {
            obstacleDistance = target.GlobalPosition.DistanceTo(InterceptionRay.GetCollisionPoint(0));
            //GD.Print("Hitting: " + (InterceptionRay.GetCollider(0) as Node3D).Name);
        }
        //else
            //GD.Print("No Hit");
        //targetDistance = obstacleDistance;

        var blendWeight = (float)Mathf.Clamp((targetDistance < currentDistance ? ZoomingInSpeed : ZoomingOutSpeed) * delta, 0, 1);
        if (obstacleDistance < ZoomDistance)
        {
            targetDistance = obstacleDistance;
            //blendWeight = 1;
        }
        else
            targetDistance = startDistance;
        currentDistance = Mathf.Lerp(currentDistance, targetDistance, blendWeight);
        currentDistance = Mathf.Clamp(currentDistance, 0f, startDistance);

        //GD.Print("Current Distance: "+ currentDistance);

        SpringLength = currentDistance;
    }

    private void evadeSurroundings(bool camera_is_controlled, float delta)
    {
        if (EvasionRay.IsColliding())
        {
            Vector3 normalHit = EvasionRay.GetCollisionNormal(0);
            Vector3 cameraForward = camera.Basis * Vector3.Forward;
            Vector3 normalHitFlat = new Vector3(normalHit.X, 0f, normalHit.Z);
            Vector3 cameraForwardFlat = new Vector3(cameraForward.X, 0f, cameraForward.Z);
            cameraTargetSpeed = SurroundingSpeed;

            float normal_influence_h = -normalHitFlat.Dot(cameraForwardFlat);
            float normal_influence_v = normalHit.Y;
            if (camera_is_controlled)
            {
                normal_influence_h *= ControlMultiplier;
                normal_influence_v *= ControlMultiplier;
            }

            rotation_degrees = processRotationInput(Vector2.Zero, new Vector2(normal_influence_h, normal_influence_v), rotation_degrees, delta);
        }
        //else
        //    GD.Print("No Hit");
    }

    private void updateOffset(float rot_difference, float deltaTime)
    {
        float dir = rot_difference > 0f ? -1f : 1f;
        if(Mathf.Abs(rot_difference) >= OffsetAmount)
            camera.HOffset = Mathf.Lerp(camera.HOffset, OffsetAmount * dir, deltaTime * OffsetSpeed);
        else
            camera.HOffset = Mathf.Lerp(camera.HOffset, 0f, deltaTime * OffsetSpeed);
    }

    private Vector3 returnRotation(Vector3 rotation_degrees, float deltaTime)
    {
        rotation_degrees.X = Mathf.Lerp(rotation_degrees.X, ReturnXAngle, deltaTime * cameraSpeed);
        rotation_degrees.X = Mathf.Clamp(rotation_degrees.X, VerticalAngleLimit.X, VerticalAngleLimit.Y);
        rotation_degrees.Y = Mathf.Lerp(rotation_degrees.Y, target.RotationDegrees.Y + ReturnYAngle, deltaTime * cameraSpeed);
        rotation_degrees.Y = Mathf.Wrap(rotation_degrees.Y, 0f, 360f);
        return rotation_degrees;
    }

    private Vector3 processRotationInput(Vector2 cam_input, Vector2 stick_input, Vector3 rotation_degrees, float deltaTime)
    {
        rotation_degrees.X = Mathf.Lerp(rotation_degrees.X, rotation_degrees.X - (deltaTime * cam_input.Y * mouse_sensitivity + stick_input.Y * stick_sensitivity), deltaTime * cameraSpeed);
        rotation_degrees.X = Mathf.Clamp(rotation_degrees.X, VerticalAngleLimit.X, VerticalAngleLimit.Y);
        rotation_degrees.Y = Mathf.Lerp(rotation_degrees.Y, rotation_degrees.Y - (deltaTime* cam_input.X * mouse_sensitivity + stick_input.X * stick_sensitivity), deltaTime * cameraSpeed);
        rotation_degrees.Y = Mathf.Wrap(rotation_degrees.Y, 0f, 360f);
        return rotation_degrees;
    }
}
