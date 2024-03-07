using Godot;
using MEC;
using System;

public partial class MimicDetection : Area3D
{
    [Export] public bool SeesTarget = false;
    [Export] public string[] TargetGroups;
    [Export] public string PriorityTargetGroup = "balloon";
    [Export] public string SoundGroup = "sound";
    [Export] public float HearingDistance = 8.0f;
    [Export] public float ForgetTime = 3.5f;

    [ExportGroup("Raycast")]
    [Export] public Marker3D StartPoint;
    [Export] public float VisionDistance = 12f;
    [Export(PropertyHint.Layers3DPhysics)] public int VisionMask = 0;

    public Node3D Target;

    private void PeriodicSoundCheck()
    {
        if(!IsInstanceValid(Target)) Target = null;
        if (Target == null)
        {
            var sounds = GetTree().GetNodesInGroup(SoundGroup);
            foreach (var item in sounds)
            {
                Node3D item3D = item as Node3D;
                if (item3D.GlobalPosition.DistanceTo(GlobalPosition) <= HearingDistance)
                {
                    Target = item3D;
                    break;
                }
            }
        }
    }

    public void PeriodicTargetLostCheck()
    {
        if ((!IsInstanceValid(Target) || Target != null) && !SeesTarget)
        {
            Target = null;
            GD.Print("Forgot Target");
        }
        else
            Timing.CallDelayed(ForgetTime, PeriodicTargetLostCheck);
    }

    public override void _Ready()
    {
        // Max value so that it will never stop until the node with the method is gone
        Timing.CallPeriodically(double.MaxValue, 0.5f, PeriodicSoundCheck);
    }

    public override void _PhysicsProcess(double delta)
    {
        if (IsInstanceValid(Target) && Target != null && SeesTarget)
        {
            float targetDist = (Target.GlobalPosition - StartPoint.GlobalPosition).Length();
            if (lookAtPlayer() < targetDist)
            {
                SeesTarget = false;
                GD.Print("Target lost");
                Timing.CallDelayed(ForgetTime, PeriodicTargetLostCheck);
            }
        }
    }

    private float lookAtPlayer()
    {
        var intersection = GetWorld3D().DirectSpaceState.IntersectRay(new PhysicsRayQueryParameters3D()
        {
            CollideWithAreas = false,
            CollideWithBodies = true,
            CollisionMask = ~(uint)VisionMask,
            From = StartPoint.GlobalPosition,
            To = Target.GlobalPosition,
            HitBackFaces = true,
            HitFromInside = false
        });
        if (intersection.Count > 0)
        {
            var hitPosition = intersection["position"].AsVector3();
            return (hitPosition - StartPoint.GlobalPosition).Length();
        }

        return VisionDistance;
    }

    public void OnBodyEnter(Node3D body)
    {
        GD.Print("Body is: " + body.Name);
        foreach (string group in TargetGroups)
        {
            if (body.IsInGroup(group) && Target == null)
            {
                SeesTarget = true;
                if (body is Node3D) Target = body as Node3D;
            }
            if(Target != null && body.IsInGroup(PriorityTargetGroup))
                if (body is Node3D) Target = body as Node3D;
        }
    }

    //public void OnBodyExit(Node3D body)
    //{
    //    if (body == Target)
    //        SeesTarget = false;
    //        Target = null;
    //    }
    //    GD.Print("Sees target: " + SeesTarget);
    //}
}
