using Godot;
using System;

public partial class Relic : CharacterBody3D
{
    [ExportCategory("Item Properties")]
    [Export] public bool IsGrabbed = false;
    [Export] public Loot loot;
    [Export] public MagicEffect effect;
    [Export] public bool Used = false;
    [Export] public float UsedTimeout = 1.0f;
    [ExportCategory("Physics")]
    [ExportGroup("Grounding")]
    [Export] public float Friction = 0.12f;
    [Export] public float FrictionLimit = 0.05f;
    [Export] public float RotationSpeed = 5f;
    [ExportGroup("Bounce")]
    [Export] public float BounceAmount = 0.95f;
    [Export] public float BounceThreshold = 0.03f;
    [ExportGroup("Gravity")]
    [Export] public float Gravity = 1.0f;
    [ExportCategory("Sounds")]
    [Export] public SoundManager Manager;
    [Export] public AudioStream ItemSound;

    private bool is_colliding_floor = false;
    private bool has_collided_floor = false;
    private bool is_colliding_wall = false;
    private bool has_collided_wall = false;

    private bool has_been_grabbed = false;

    private Vector3 originalRotation = Vector3.Zero;

    private Vector3 velocity = Vector3.Zero;

    public override void _Ready()
    {
        originalRotation = GlobalRotationDegrees;
    }

    private void setCollisionFrame()
    {
        if (IsOnFloor())
        {
            if (!has_collided_floor && !is_colliding_floor)
            {
                is_colliding_floor = true;
                has_collided_floor = true;
            }
            else if (has_collided_floor && is_colliding_floor) is_colliding_floor = false;
        }
        else
        {
            has_collided_floor = false;
            is_colliding_floor = false;
        }

        if (IsOnWall())
        {
            if (!has_collided_wall && !is_colliding_wall)
            {
                is_colliding_wall = true;
                has_collided_wall = true;
            }
            else if (has_collided_wall && is_colliding_wall) is_colliding_wall = false;
        }
        else
        {
            has_collided_wall = false;
            is_colliding_wall = false;
        }
    }

    public void IsUsed()
    {
        Used = true;
        Timer timer = new Timer();
        timer.Connect("timeout", new Callable(this, "Despawn"));
        timer.WaitTime = UsedTimeout;
        timer.Autostart = true;
        timer.OneShot = true;
        this.AddChild(timer);
        Manager.PlayRandom("Use", minpitch: 0.95f, maxpitch: 1.05f);
        Manager.PlayOneShot(ItemSound, minpitch: 0.95f, maxpitch: 1.05f);
    }

    private void Despawn()
    {
        // Particles
        Manager.PlayRandom("Break", minpitch: 0.95f, maxpitch: 1.05f);
        Manager.PlayOneShot(ItemSound, minpitch: 0.95f, maxpitch: 1.05f);
        QueueFree();
    }

    public void SetVelocity(Vector3 velocity) {
        this.velocity = velocity;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (!IsGrabbed)
        {
            has_been_grabbed = false;
            setCollisionFrame();

            // Add the gravity.
            if (!IsOnFloor())
                velocity.Y -= Gravity * (float)delta;
            else
            {
                float friction_reduction = 1f - Friction;
                velocity = new Vector3(velocity.X * friction_reduction, velocity.Y, velocity.Z * friction_reduction);
                if (velocity.Length() <= FrictionLimit) velocity = Vector3.Zero;
            }

            GlobalRotationDegrees = GlobalRotationDegrees.Lerp(originalRotation, RotationSpeed * (float)delta);

            if (is_colliding_floor || is_colliding_wall)
            {
                Vector3 bounce = Vector3.Zero;

                if (IsOnFloor() && Math.Abs(velocity.Y) >= BounceThreshold)
                {
                    velocity.Y = velocity.Bounce(GetFloorNormal()).Y * BounceAmount;
                }
                if (IsOnWall() && new Vector2(velocity.X, velocity.Z).Length() >= BounceThreshold)
                {
                    velocity = velocity.Bounce(GetWallNormal()) * BounceAmount;
                }
                Manager.PlayRandom("Collide", minpitch: 0.95f, maxpitch: 1.05f);
                Manager.PlayOneShot(ItemSound, minpitch: 0.95f, maxpitch: 1.05f);
                //GD.Print("Speed is "+ velocity.Length());
            }

            Velocity = velocity;
            MoveAndSlide();
        }
        else
        {
            Velocity = Vector3.Zero;
            velocity = Vector3.Zero;
            if (!has_been_grabbed)
            {
                Manager.PlayRandom("Grab", minpitch: 0.95f, maxpitch: 1.05f);
                Manager.PlayOneShot(ItemSound, minpitch: 0.95f, maxpitch: 1.05f);
                has_been_grabbed = true;
            }
        }
    }
}
