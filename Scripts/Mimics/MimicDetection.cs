using Godot;
using System;

public partial class MimicDetection : Area3D
{
    [Export] public bool SeesTarget = false;
    [Export] public string[] TargetGroups;
    [Export] public string PriorityTargetGroup = "balloon";

    public Node3D Target;

    public void OnBodyEnter(Node3D body)
    {
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
