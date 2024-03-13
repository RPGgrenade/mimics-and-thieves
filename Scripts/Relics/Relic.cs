using Godot;
using System;
using System.Linq;

public partial class Relic : CharacterBody3D
{
    [Export] public bool IsCoin = false;
    [ExportCategory("Item Properties")]
    [Export] public bool IsGrabbed = false;
    [Export] public Loot loot;
    [Export] public MagicEffect effect;
    [Export] public bool Used = false;
    [Export] public float UsedTimeout = 1.0f;
    [ExportCategory("Physics")]
    [Export] public float PhysicsDistanceFromCamera = 30f; // Distance at which most physics applies
    [ExportGroup("Grounding")]
    [Export] public float Friction = 0.12f;
    [Export] public float FrictionLimit = 0.05f;
    [Export] public float RotationSpeed = 5f;
    [Export] public float DerotateSpeed = 3f;
    [ExportGroup("Bounce")]
    [Export] public float BounceAmount = 0.95f;
    [Export] public float BounceThreshold = 0.03f;
    [Export] public float BounceRotationAdd = 900f;
    [ExportGroup("Gravity")]
    [Export] public float Gravity = 1.0f;
    [ExportCategory("Sounds")]
    [Export] public float SoundDistanceFromCamera = 35f; // Distance at which sounds will play
    [Export] public PackedScene Manager;
    [Export] public float GrabSoundVolume = -3f;
    [Export] public float UseSoundVolume = 5f;
    [Export] public float BreakSoundVolume = 0f;
    [Export] public float CollideSoundVolume = 5f;
    [Export] public AudioStream ItemSound;
    [Export] public float ItemSoundVolume = 0f;

    private bool is_colliding_floor = false;
    private bool has_collided_floor = false;
    private bool is_colliding_wall = false;
    private bool has_collided_wall = false;

    private bool has_been_grabbed = false;

    private Vector3 originalRotation = Vector3.Zero;
    private Vector3 extraRotation = Vector3.Zero;

    private Vector3 velocity = Vector3.Zero;

    private Node3D camera;
    private float distanceFromCamera = 0f;

    public override void _Ready()
    {
        originalRotation = GlobalRotationDegrees;
        camera = GetViewport().GetCamera3D();
        if (IsCoin)
            RotationDegrees += new Vector3(
                (float)GD.RandRange(-BounceRotationAdd, BounceRotationAdd), 
                (float)GD.RandRange(-BounceRotationAdd, BounceRotationAdd),
                (float)GD.RandRange(-BounceRotationAdd, BounceRotationAdd)
            );
        if(loot != null)
            CarryData.Instance.RemainingLootValue += loot.GetValue();
    }

    private void PlaySounds(string soundType, float minPitch = 0.95f, float maxPitch = 1.05f, float volume = 0f, float generic_volume = 0f, bool hearable = false)
    {
        if (Manager != null && distanceFromCamera <= SoundDistanceFromCamera)
        {
            SoundManager sounds = Manager.Instantiate() as SoundManager;
            if(!hearable)
                sounds.RemoveFromGroup("sound");
            GetTree().Root.AddChild(sounds);
            sounds.GlobalPosition = GlobalPosition;


            sounds.PlayRandom(soundType, minpitch: minPitch, maxpitch: maxPitch, volume: generic_volume);
            sounds.PlayOneShot(ItemSound, minpitch: minPitch, maxpitch: maxPitch, volume: volume);
        }
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
        PlaySounds("Use", volume: ItemSoundVolume, generic_volume: UseSoundVolume);
    }

    private void Despawn()
    {
        GD.Print("Used: "+Used);
        if (Used)
        {
            // Particles
            PlaySounds("Break", volume: ItemSoundVolume, generic_volume: BreakSoundVolume);
            if(loot != null)
                CarryData.Instance.RemainingLootValue -= loot.GetValue();
            QueueFree();
        }
    }

    public void SetVelocity(Vector3 velocity) {
        this.velocity = velocity;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (camera != null && IsInstanceValid(camera))
            distanceFromCamera = GlobalPosition.DistanceTo(camera.GlobalPosition);
        if (!IsGrabbed)
        {
            has_been_grabbed = false;
            setCollisionFrame();

            // Add the gravity.
            if (!IsOnFloor())
                velocity.Y -= Gravity * (float)delta;
            else
            {
                if (distanceFromCamera <= PhysicsDistanceFromCamera)
                {
                    float friction_reduction = 1f - Friction;
                    velocity = new Vector3(velocity.X * friction_reduction, velocity.Y, velocity.Z * friction_reduction);
                    if (velocity.Length() <= FrictionLimit) velocity = Vector3.Zero;
                }
            }

            GlobalRotationDegrees = GlobalRotationDegrees.Lerp(originalRotation + extraRotation, RotationSpeed * (float)delta);
            extraRotation = extraRotation.Lerp(Vector3.Zero, (float)delta * DerotateSpeed);

            if (distanceFromCamera <= PhysicsDistanceFromCamera && (is_colliding_floor || is_colliding_wall))
            {
                Vector3 bounce = Vector3.Zero;

                if (IsOnFloor() && Math.Abs(velocity.Y) >= BounceThreshold)
                {
                    extraRotation += new Vector3(
                        (float)GD.RandRange(-BounceRotationAdd, BounceRotationAdd),
                        (float)GD.RandRange(-BounceRotationAdd, BounceRotationAdd),
                        (float)GD.RandRange(-BounceRotationAdd, BounceRotationAdd)
                    );
                    velocity.Y = velocity.Bounce(GetFloorNormal()).Y * BounceAmount;
                }
                if (IsOnWall() && new Vector2(velocity.X, velocity.Z).Length() >= BounceThreshold)
                {
                    extraRotation += new Vector3(
                        (float)GD.RandRange(-BounceRotationAdd, BounceRotationAdd),
                        (float)GD.RandRange(-BounceRotationAdd, BounceRotationAdd),
                        (float)GD.RandRange(-BounceRotationAdd, BounceRotationAdd)
                    );
                    velocity = velocity.Bounce(GetWallNormal()) * BounceAmount;
                }
                PlaySounds("Collide", volume: ItemSoundVolume, generic_volume: CollideSoundVolume, hearable : true);
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
                PlaySounds("Grab", volume: ItemSoundVolume, generic_volume: GrabSoundVolume);
                has_been_grabbed = true;
            }
        }
    }
}
