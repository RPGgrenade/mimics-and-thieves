using Godot;
using System;
using System.Linq;

public partial class Mimic : CharacterBody3D
{
    [Export] public bool IsMimic = true;
    [Export] public float ActivityDistanceFromCamera = 25f;
    [Export] public Node3D Glow;
    [ExportCategory("References")]
    [Export] public MimicDetection Detection;
    [Export] public MimicAnimator Animator;
    [ExportCategory("Navigation")]
    [Export] public NavigationAgent3D Navigator;
    [Export] public NavigationObstacle3D Obstacle;
    [Export] public Node3D Target;
    [ExportCategory("Movement")]
    [Export] public float MaxSpeed = 6f;
    [Export] public float JumpVelocity = 2.5f;
    [Export] public float Acceleration = 12f;
    [Export] public float RotationSpeed = 8f;
    [Export] public float Gravity = 9.8f;
    [Export] public float JumpCooldown = 0.1f;
    [ExportCategory("Sounds")]
    [Export] public SoundManager Manager;
    [Export] public AudioStream LandSound;

    private float _current_rotation_difference = 1.0f;

    private float jumpCooldown = 0f;

    private Node3D camera;
    private float distanceFromCamera = float.MaxValue;

    public override void _Ready()
    {
        Navigator = Navigator ?? GetChildren().OfType<NavigationAgent3D>().FirstOrDefault();
        Obstacle = Obstacle ?? GetChildren().OfType<NavigationObstacle3D>().FirstOrDefault();
        if (!IsMimic && Glow != null)
        {
            Glow.QueueFree();
            Detection.QueueFree();
            Navigator.QueueFree();
            Animator.QueueFree();
        }
        else
            Obstacle.QueueFree();
        if(IsMimic)
            camera = GetViewport().GetCamera3D();
    }

    public override void _Process(double delta)
    {
        if (IsMimic)
        {
            if (camera != null && IsInstanceValid(camera))
                distanceFromCamera = GlobalPosition.DistanceTo(camera.GlobalPosition);
            //GD.Print("Distance from camera: " + distanceFromCamera);
        }
        if (IsMimic && distanceFromCamera <= ActivityDistanceFromCamera)
        {
            Target = Detection.Target;
            if (Target != null && IsInstanceValid(Target))
            {
                if (!Animator.Active) Animator.Activate();
                MoveBody(Target.GlobalPosition, MaxSpeed, Acceleration, RotationSpeed, (float)delta);
            }
            else
            {
                if (!IsInstanceValid(Target)) Target = null;
                if (Animator.Active)
                {
                    Animator.Deactivate(); // For now. Needs a cooldown time
                    MoveBody(GlobalPosition, MaxSpeed, Acceleration, RotationSpeed, (float)delta);
                    Velocity = Vector3.Zero;
                }
            }
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        if (IsMimic && distanceFromCamera <= ActivityDistanceFromCamera)
        {
            Vector3 velocity = Velocity;
            if (!IsOnFloor())
                velocity.Y -= Gravity * (float)delta;
            else
            {
                if (jumpCooldown > 0f) jumpCooldown -= (float)delta;
                else { 
                    jumpCooldown = JumpCooldown;
                    if(Animator.Active)
                        Manager.PlayOneShot(LandSound, volume: -10f);
                }

                if (Target != null)
                {
                    if (jumpCooldown <= 0f)
                        velocity.Y = JumpVelocity;
                    else velocity = Vector3.Zero;
                }
            }
            Velocity = velocity;
            MoveAndSlide();

            if(GetSlideCollisionCount() > 0 && !IsOnFloor())
            {
                //GD.Print("Hit Something");
                Node3D collision = GetLastSlideCollision().GetCollider() as Node3D;
                if (collision.IsInGroup("player") && collision is ThiefController)
                {
                    //GD.Print("Hit player");
                    ThiefController thief = collision as ThiefController;
                    if (thief.GetIFrames() <= 0f && !thief.IsProtected && !thief.IsDodging)
                    {
                        //GD.Print("Hurt player");
                        thief.SetIFrames();
                        thief.loot.LoseLoot();
                        Loot selected = thief.loot.SelectedLoot;
                        if (selected != null)
                            thief.UI.UpdateSelectedItem(
                                selected?.name ?? "",
                                thief.loot.SelectedCount,
                                selected.magic?.name ?? "",
                                selected.UI ?? null
                            );
                    }
                }
            }
        }
    }

    public void MoveBody(Vector3 TargetPosition, float MoveSpeed, float Acceleration, float RotationSpeed, float delta)
    {
        Vector3 direction = new Vector3();

        Navigator.TargetPosition = TargetPosition;
        Navigator.MaxSpeed = MoveSpeed;

        // Get Movement direction
        direction = Navigator.GetNextPathPosition() - GlobalPosition;
        direction = direction.Normalized();
        direction = new Vector3(direction.X, 0.0f, direction.Z);

        // Properly set look rotation
        Vector2 look_direction = new Vector2(direction.Z, direction.X);
        Vector3 rotation = Rotation;
        _current_rotation_difference = (180.0f - Mathf.RadToDeg(Math.Abs(rotation.Y - look_direction.Angle()))) / 180.0f;
        rotation.Y = Mathf.LerpAngle(rotation.Y, look_direction.Angle(), delta * RotationSpeed);
        Rotation = rotation;

        // Move
        Vector3 velocity = Velocity.Lerp(direction * MoveSpeed * Math.Abs(_current_rotation_difference), Acceleration * delta);
        Velocity = new Vector3(velocity.X, Velocity.Y, velocity.Z);
    }
}
