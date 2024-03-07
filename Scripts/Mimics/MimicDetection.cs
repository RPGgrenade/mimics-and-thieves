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

    public Node3D Target;

    private void PeriodicCheck()
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

    public override void _Ready()
    {
        // Max value so that it will never stop until the node with the method is gone
        Timing.CallPeriodically(double.MaxValue, 0.5f, PeriodicCheck);
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
        GD.Print("Sees target: "+SeesTarget);
    }

    public void OnBodyExit(Node3D body)
    {
        if (body == Target)
        {
            SeesTarget = false;
            Target = null;
        }
        GD.Print("Sees target: " + SeesTarget);
    }
}
