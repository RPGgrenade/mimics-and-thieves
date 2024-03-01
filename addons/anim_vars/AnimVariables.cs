using Godot;
using Godot.Collections;
using System.ComponentModel;
using System.Linq;

[GodotClassName("AnimationVariables")]
public partial class AnimVariables : Node
{
    [Export] public AnimationTree AnimTree;
    [ExportCategory("Variables")]
    [Export] public AnimVariable[] Variables = new AnimVariable[0];
    [Export] public AnimVariableBranch[] Branches = new AnimVariableBranch[0];
    [Export] public AnimScalarExpression[] Scalars = new AnimScalarExpression[0];

    private Dictionary<string, AnimVariable> variables = new Dictionary<string, AnimVariable>();

    public override void _Ready()
    {
        if (GetParent() is AnimationTree)
        {
            AnimTree = AnimTree ?? GetParent<AnimationTree>();
            AnimTree.AdvanceExpressionBaseNode = GetPath();
        }

        foreach (AnimVariable variable in Variables)
            variables.Add(variable.variableName, variable);
        foreach(AnimVariableBranch branch in Branches)
        {
            if (branch.Register)
            {
                foreach (AnimVariable variable in branch.Variables)
                    variables.Add(variable.variableName, variable);
            }
        }
    }

    public override void _Process(double delta)
    {
        processScalars();
    }

    private void processScalars()
    {
        foreach (AnimScalarExpression scalar in Scalars)
        {
            Expression exp = new Expression();
            Error err = exp.Parse(scalar.Expression);
            if (err == Error.Ok)
            {
                Variant result = exp.Execute(baseInstance: this);
                if (!exp.HasExecuteFailed())
                {
                    StringName property = new StringName(scalar.ScalarPath);
                    AnimTree.Set(property, result);
                }
                else
                    GD.PrintErr("Expression '"+scalar.Expression+"' not valid.\nPerhaps you're missing a string for the get function.");
            }
            else
                GD.PrintErr(err);
        }
    }

    public Variant Get(string varName)
    {
        if(variables.ContainsKey(varName))
            return variables[varName].Value;
        return 0;
    }

    public void Set(string varName, Variant value)
    {
        if (variables.ContainsKey(varName))
            variables[varName].Value = value;
        else
            GD.PrintErr("Variable "+varName+" isn't in this list of variables");
    }
}
