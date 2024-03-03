using Godot;
using System;

public partial class ThiefController : CharacterBody3D
{
    [ExportCategory("Scripts")]
    [Export] public ThiefInputs inputs;
    [Export] public AnimVariables variables;
    [Export] public Inventory loot;
    [Export] public Grab grab;

    [ExportCategory("Movement")]
    [ExportGroup("Ground")]
    [Export] private bool IsRunning = false;
    [Export] private bool IsDodging = false;
    [ExportSubgroup("Walking")]
    [Export] private float WalkingAcceleration = 2.0f;
    [Export] private float WalkingDecceleration = 14.0f;
    [Export] private float WalkingMaxSpeed = 3.5f;
    [Export] private float WalkingRotationDifferenceSpeedMultiplier = 3.0f;
    [Export] private float WalkingRotationSpeed = 8.0f;
    [ExportSubgroup("Running")]
    [Export] private float RunningAcceleration = 8.0f;
    [Export] private float RunningDecceleration = 8.0f;
    [Export] private float RunningMaxSpeed = 6.0f;
    [Export] private float RunningRotationDifferenceSpeedMultiplier = 1.2f;
    [Export] private float RunningRotationSpeed = 3.0f;
    [ExportSubgroup("Dodging")]
    [Export] private float DodgingAcceleration = 50.0f;
    [Export] private float DodgingDecceleration = 5.0f;
    [Export] private float DodgingMaxSpeed = 15.0f;
    [Export] private float DodgingRotationSpeed = 10.0f;
    [ExportGroup("Air")]
    [Export] private float JumpVelocity = 4.5f;
    [Export] private float Gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

    [ExportGroup("Shoot")]
    [Export] private float ShootCooldown = 0.1f;

    [ExportCategory("Camera")]
    [Export] private Node3D Camera;

    private Vector3 _velocity = Vector3.Zero;
    private Vector3 _snap_vector = Vector3.Down;
    private float _current_speed = 0.0f;
    private float _current_rotation_difference = 1.0f;

    private float _shoot_cooldown = 0.0f;

    private Vector3 momentum = Vector3.Zero;
    private Vector3 move_direction = Vector3.Zero;
    private Vector3 stored_move_direction = Vector3.Zero;

    public override void _Process(double delta)
    {
        if (Camera is Node3D)
        {
            Camera.Position = Position;
        }

        if (variables != null)
        {
            Vector3 hor_speed = new Vector3(Velocity.X, 0, Velocity.Z);

            variables.Set("HorizontalSpeed", hor_speed.Length());
            variables.Set("VerticalSpeed", Velocity.Y);
            variables.Set("OnGround", IsOnFloor());
            variables.Set("Jump", inputs.Jump);
            variables.Set("Dodge", inputs.Dodge);

            variables.Set("Grab", inputs.Grab);
            variables.Set("Bag", inputs.Bag);
            variables.Set("Swing", inputs.Swing);

            variables.Set("JustGrabbed", inputs.JustGrabbed);
        }

        if (inputs.CycleLeft) loot.CycleLeft();
        if (inputs.CycleRight) loot.CycleRight();
    }

    public void SlowDodge() {
        move_direction = Vector3.Zero;
    }

    public override void _PhysicsProcess(double delta)
    {
        IsRunning = inputs.Run;
        if (momentum == Vector3.Zero)
        {
            move_direction = Vector3.Zero;
            // Get inputs and create rotation based on camera (spring) rotation
            float move_dir_amount = inputs.LeftStick.Length();
            move_direction.X = inputs.LeftStick.X;
            move_direction.Z = inputs.LeftStick.Y;
            move_direction = move_direction.Normalized().Rotated(Vector3.Up, Camera.Rotation.Y).Normalized() * move_dir_amount;
            stored_move_direction = move_direction;
        }
        if (IsDodging)
        {
            accelerationControl(move_direction, (float)delta,
                DodgingAcceleration,
                DodgingDecceleration,
                DodgingMaxSpeed
            );
            setVelocityValues(move_direction, (float)delta, true);
            jumping(JumpVelocity);
            rotating((float)delta, DodgingRotationSpeed);
            Velocity = momentum;
        }
        else
        {
            momentum = Vector3.Zero;
            accelerationControl(move_direction, (float)delta,
                IsRunning ? RunningAcceleration : WalkingAcceleration,
                IsRunning ? RunningDecceleration : WalkingDecceleration,
                IsRunning ? RunningMaxSpeed : WalkingMaxSpeed
            );
            setVelocityValues(move_direction, (float)delta);
            jumping(JumpVelocity);
            rotating((float)delta, IsRunning ? RunningRotationSpeed : WalkingRotationSpeed);
            rotationDifferenceVelocityChange(IsRunning ? RunningRotationDifferenceSpeedMultiplier : WalkingRotationDifferenceSpeedMultiplier);
            Velocity = _velocity;
        }
        MoveAndSlide();
    }

    private void accelerationControl(Vector3 move_direction, float delta, float acceleration, float decceleration, float maxSpeed)
    {
        // Set up speed acceleration and decceleration
        _current_speed += (move_direction.Length() != 0 ? acceleration : -decceleration) * delta;
        _current_speed = Math.Clamp(_current_speed, 0.0f, maxSpeed);
    }

    private void setVelocityValues(Vector3 move_direction, float delta, bool use_momentum = false)
    {
        // Set velocity values with input and speeds
        if (!use_momentum)
        {
            _velocity.X = move_direction.X * _current_speed;
            _velocity.Z = move_direction.Z * _current_speed;
        }
        else
        {
            momentum.X = stored_move_direction.X * _current_speed;
            momentum.Z = stored_move_direction.Z * _current_speed;
        }
        _velocity.Y = !IsOnFloor() ? _velocity.Y - Gravity * delta : _velocity.Y;
    }

    private void jumping(float jumpVelocity)
    {
        // Jumping logic
        bool just_landed = IsOnFloor() && _snap_vector == Vector3.Zero;
        bool is_jumping = IsOnFloor() && inputs.Jump;
        if (is_jumping)
        {
            _velocity.Y = jumpVelocity;
            _snap_vector = Vector3.Zero;
        }
        else _snap_vector = Vector3.Down;
        FloorSnapLength = _snap_vector.Length();
    }

    private void rotating(float delta, float rotationSpeed)
    {
        // Rotation logic
        Vector2 look_direction = new Vector2(_velocity.Z, _velocity.X);
        if (look_direction.Length() > 0.4f)
        {
            Vector3 rotation = Rotation;
            _current_rotation_difference = (180.0f - Mathf.RadToDeg(Math.Abs(rotation.Y - look_direction.Angle()))) / 180.0f;
            rotation.Y = Mathf.LerpAngle(rotation.Y, look_direction.Angle(), delta * rotationSpeed);
            Rotation = rotation;
        }
    }

    private void rotationDifferenceVelocityChange(float rotationDifferenceSpeedMultiplier)
    {
        // Rotation difference affecting speed
        float difference_speed_modifier = Math.Abs(_current_rotation_difference) / rotationDifferenceSpeedMultiplier;
        if (difference_speed_modifier <= 0.8f)
            _velocity *= new Vector3(difference_speed_modifier, 1f, difference_speed_modifier);
    }
}
