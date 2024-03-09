using Godot;
using System;

public partial class ThiefController : CharacterBody3D
{
    [ExportCategory("Scripts")]
    [Export] public ThiefInputs inputs;
    [Export] public AnimVariables variables;
    [Export] public Node3D Undetect;
    [Export] public Inventory loot;
    [Export] public Grab grab;
    [Export] public ThiefUI UI;

    [ExportCategory("Movement")]
    [ExportGroup("Ground")]
    [Export] public bool IsRunning = false;
    [Export] public bool IsDodging = false;
    [Export] public bool IsProtected = false;
    [Export] public bool IsUndetectable = false;
    [Export] public float UndetectSpeed = 4f;
    [Export] private float MomentumReducitonSpeed = 15.0f;
    [Export] private float MomentumAirReducitonSpeed = 3.0f;
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
    [Export] private float CoyoteTime = 0.1f; //6 Frames
    [Export] private float DodgeCoyoteTime = 1.0f; //6 Frames
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

    private float _coyote_time = 0.0f;

    private Vector3 momentum = Vector3.Zero;
    private Vector3 move_direction = Vector3.Zero;
    private Vector3 stored_move_direction = Vector3.Zero;

    private Vector3 undetectSize = Vector3.Zero;

    public override void _Ready()
    {
        undetectSize = Undetect.Scale;
        Undetect.Scale = Vector3.Zero;
    }

    public override void _Process(double delta)
    {

        if (inputs.Pause)
        {
            UI.Paused = !UI.Paused;
            UI.UpdateInventory(loot._allLoot);
        }

        if (!UI.Paused)
        {
            if (Camera is Node3D)
            {
                Camera.Position = Position + (Vector3.Up * 1.6f);
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

            if (inputs.CycleLeft)
            {
                loot.CycleLeft();
                Loot selected = loot.SelectedLoot;
                if (selected != null)
                    UI.UpdateSelectedItem(
                        selected?.name ?? "",
                        loot.SelectedCount,
                        selected.magic?.name ?? "",
                        selected.UI ?? null
                    );
            }
            if (inputs.CycleRight)
            {
                loot.CycleRight();
                Loot selected = loot.SelectedLoot;
                if (selected != null)
                    UI.UpdateSelectedItem(
                        selected?.name ?? "",
                        loot.SelectedCount,
                        selected.magic?.name ?? "",
                        selected.UI ?? null
                    );
            }

            Undetect.Scale = Undetect.Scale.Lerp(IsUndetectable ? undetectSize : Vector3.Zero, (float)delta * UndetectSpeed);
        }
    }

    public void SlowDodge() {
        move_direction = Vector3.Zero;
    }

    public void SetUndetectable(float duration)
    {
        IsUndetectable = true;
        Timer timer = new Timer();
        timer.Connect("timeout", new Callable(this, "Detectable"));
        timer.WaitTime = duration;
        timer.Autostart = true;
        timer.OneShot = true;
        this.AddChild(timer);
    }

    private void Detectable()
    {
        // Particles
        // Sound
        IsUndetectable = false;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (!UI.Paused)
        {
            IsRunning = inputs.Run;
            if (IsOnFloor())
            {
                stored_move_direction = stored_move_direction.Lerp(move_direction, (float)delta * 5f);
                _coyote_time = IsDodging ? DodgeCoyoteTime : CoyoteTime;
            }
            else
                _coyote_time -= (float)delta;
            if (momentum.Length() <= 0.1f || !IsOnFloor())
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
                momentum = momentum.Lerp(Vector3.Zero,(float)delta * (IsOnFloor() ? MomentumReducitonSpeed : MomentumAirReducitonSpeed));
                accelerationControl(move_direction, (float)delta,
                    IsRunning ? RunningAcceleration : WalkingAcceleration,
                    IsRunning ? RunningDecceleration : WalkingDecceleration,
                    IsRunning ? RunningMaxSpeed : WalkingMaxSpeed
                );
                setVelocityValues(move_direction, (float)delta);
                jumping(JumpVelocity);
                rotating((float)delta, IsRunning ? RunningRotationSpeed : WalkingRotationSpeed);
                rotationDifferenceVelocityChange(IsRunning ? RunningRotationDifferenceSpeedMultiplier : WalkingRotationDifferenceSpeedMultiplier);
                Velocity = (_velocity + momentum);
            }
            //GD.Print("Momentum: " + momentum);
            //GD.Print("Velocity: " + _velocity);
            MoveAndSlide();
        }
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
        //else if (!use_momentum && !IsOnFloor())
        //{
        //    momentum.X = stored_move_direction.X * _current_speed/2f;
        //    momentum.Z = stored_move_direction.Z * _current_speed/2f;
        //    _velocity.X = move_direction.X * _current_speed;
        //    _velocity.Z = move_direction.Z * _current_speed;
        //}
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
        bool just_landed = (IsOnFloor() || _coyote_time > 0f) && _snap_vector == Vector3.Zero;
        bool is_jumping = (IsOnFloor() || _coyote_time > 0f) && inputs.Jump;
        if (is_jumping)
        {
            _velocity.Y = IsDodging ? 0f : jumpVelocity;
            momentum.Y = IsDodging ? jumpVelocity : 0f;
            _snap_vector = Vector3.Zero;
            momentum.X = stored_move_direction.X * _current_speed;
            momentum.Z = stored_move_direction.Z * _current_speed;
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
