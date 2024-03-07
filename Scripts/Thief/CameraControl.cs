using Godot;
using Godot.Collections;

public partial class CameraControl : Node
{
    private Camera3D _camera;
    [ExportGroup("Scripts")]
    [Export]
    public ThiefInputs inputs;
    [ExportGroup("Camera Target")]
    [Export]
    public Node3D Target;
    [Export]
    public float OrbitDistance;
    [Export(PropertyHint.Range, "0,10,0.01")]
    public float LazinessYaw;
    [Export(PropertyHint.Range, "0,10,0.01")]
    public float LazinessPitch;

    [ExportGroup("Camera Whiskers")]
    [Export]
    public bool UseWhiskers = true;
    [Export]
    public Shape3D WhiskerShape;
    [Export]
    public float MinWhiskerStep = 0.5f;

    [ExportGroup("Camera Blending")]
    [Export]
    public float PositionBlendSpeed = 30f;
    [Export]
    public float DistanceBlendSpeedIn = 15f;
    [Export]
    public float DistanceBlendSpeedOut = 3f;

    [ExportGroup("Camera Input")]
    [Export]
    public bool Locked = true;
    [Export]
    public Vector2 Sensitivity = new Vector2(20, 20);

    private bool _previouslyTracking = false;

    private Vector3 _angle;
    private Vector3 _angleDelta;
    private float _currentDistance;

    public override void _Ready()
    {
        _camera = GetParent<Camera3D>();
        Locked = true;
        _currentDistance = OrbitDistance;
        ProcessPhysicsPriority = 1;
    }

    private void Lock()
    {
        Locked = true;
        // GD.Print("Lock");
    }
    private void Unlock()
    {
        Locked = false;
        // GD.Print("Unlock");
    }


    public override void _PhysicsProcess(double delta)
    {
        UpdateState(delta);
    }

    private void UpdateState(double delta)
    {
        if (Target == null) return;

        Vector2 mouse_input = inputs.MouseInput;
        Vector2 stick_input = inputs.RightStick;
        _angleDelta.X += mouse_input.Y * -0.0001f * Sensitivity.X;
        _angleDelta.Y += stick_input.X * -0.0001f * Sensitivity.Y;

        var targetPos = Target.GlobalPosition;

        ApplyLazinessToAngle(targetPos);

        _angle += _angleDelta;
        _angleDelta = Vector3.Zero;

        _angle.X = Mathf.Clamp(_angle.X, -Mathf.Pi * 0.5f, Mathf.Pi * 0.5f);
        _camera.Quaternion = Quaternion.FromEuler(_angle);
        var cameraForward = _camera.Quaternion * Vector3.Forward;

        float obstacleDistance = GetTargetDistanceByRay(targetPos, cameraForward, OrbitDistance);
        float targetDistance = obstacleDistance;
        if (UseWhiskers)
            targetDistance = GetTargetDistanceByWhiskers(targetPos, cameraForward, obstacleDistance);
        // GD.Print("Distance: " + targetDistance);

        var blendWeight = (float)Mathf.Clamp((targetDistance < _currentDistance ? DistanceBlendSpeedIn : DistanceBlendSpeedOut) * delta, 0, 1);
        if (obstacleDistance < OrbitDistance && targetDistance >= obstacleDistance)
        {
            targetDistance = obstacleDistance;
            blendWeight = 1;
        }
        _currentDistance = Mathf.Lerp(_currentDistance, targetDistance, blendWeight);

        _camera.GlobalPosition = targetPos - cameraForward * _currentDistance;
    }

    private void ApplyLazinessToAngle(Vector3 targetPos)
    {
        Vector3 directionToTarget = (targetPos - _camera.GlobalPosition).Normalized();
        float xz = (directionToTarget * new Vector3(1, 0, 1)).Length();

        Vector3 anglesToTarget = new Vector3(
            Mathf.Atan2(directionToTarget.Y, xz),
            -Mathf.Atan2(directionToTarget.X, -directionToTarget.Z),
            0
        );

        _angle.X = Mathf.LerpAngle(_angle.X, anglesToTarget.X, LazinessPitch);
        _angle.Y = Mathf.LerpAngle(_angle.Y, anglesToTarget.Y, LazinessYaw);
    }

    private float GetTargetDistanceByRay(Vector3 targetPos, Vector3 cameraForward, float maxDistance, float bias = 0.1f)
    {
        var intersection = _camera.GetWorld3D().DirectSpaceState.IntersectRay(new PhysicsRayQueryParameters3D()
        {
            CollideWithAreas = false,
            CollideWithBodies = true,
            CollisionMask = ~(uint)0,
            From = targetPos,
            To = targetPos - cameraForward * maxDistance,
            HitBackFaces = true,
            HitFromInside = false
        });
        if (intersection.Count > 0)
        {
            var hitPosition = intersection["position"].AsVector3();
            return (hitPosition - targetPos).Length() - bias;
        }

        return maxDistance;
    }

    private float GetTargetDistanceByWhiskers(Vector3 targetPos, Vector3 cameraForward, float maxDistance, float bias = 0.1f)
    {
        float minDistance = 0.01f;

        while (maxDistance - minDistance > MinWhiskerStep)
        {
            float testDistance = (minDistance + maxDistance) * 0.5f;
            if (TestWhisker(targetPos, cameraForward, testDistance, bias))
            {
                maxDistance = testDistance;
            }
            else
            {
                minDistance = testDistance;
            }
        }

        return maxDistance;
    }

    private bool TestWhisker(Vector3 targetPos, Vector3 cameraForward, float distance, float bias)
    {
        float radius = distance * 0.5f;
        Vector3 center = targetPos - cameraForward * radius;
        var intersections = _camera.GetWorld3D().DirectSpaceState.IntersectShape(new PhysicsShapeQueryParameters3D()
        {
            CollideWithAreas = false,
            CollideWithBodies = true,
            CollisionMask = ~(uint)2,
            Margin = bias,
            Motion = Vector3.Zero,
            Shape = WhiskerShape,
            Transform = Transform3D.Identity.Translated(center).ScaledLocal(new Vector3(radius, radius, radius))
        });
        // GD.Print("[" + (intersections.Count > 0) + "] Testing whisker distance " + distance);
        return intersections.Count > 0;
    }
}
