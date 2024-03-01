using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using static AnimVariable;

[GodotClassName("State")]
[Tool]
public partial class State : Node
{
    [Export] public StateMachine Machine;

    [ExportCategory("State Trackers")]
    // States that can be transitioned to
    [Export] public State[] TransitionStates = new State[0];
    // Randomly picks one of the exit states if there's more than one
    [Export] public State[] ExitStates = new State[0];

    //[ExportCategory("Custom Expressions")]
    public bool UseOnEnter
    {
        get => _useOnEnter;
        set { _useOnEnter = value; NotifyPropertyListChanged(); }
    }
    public bool UseOnExit
    {
        get => _useOnExit;
        set { _useOnExit = value; NotifyPropertyListChanged(); }
    }
    public bool UseOnUpdate
    {
        get => _useOnUpdate;
        set { _useOnUpdate = value; NotifyPropertyListChanged(); }
    }
    public bool UseOnPhysicsUpdate
    {
        get => _useOnPhysicsUpdate;
        set { _useOnPhysicsUpdate = value; NotifyPropertyListChanged(); }
    }
    public string OnEnter;
    public string OnExit;
    public string OnUpdate;
    public string OnPhysicsUpdate;

    private bool _useOnEnter;
    private bool _useOnExit;
    private bool _useOnUpdate;
    private bool _useOnPhysicsUpdate;

    private Godot.Collections.Array<Godot.Collections.Dictionary> _properties;

    public override Godot.Collections.Array<Godot.Collections.Dictionary> _GetPropertyList()
    {
        _properties = new Godot.Collections.Array<Godot.Collections.Dictionary>();
        addProperty("OnEnter", Variant.Type.String, UseOnEnter ? PropertyUsageFlags.Default : PropertyUsageFlags.NoEditor, "On Enter");
        addProperty("OnExit", Variant.Type.String, UseOnExit ? PropertyUsageFlags.Default : PropertyUsageFlags.NoEditor, "On Exit");
        addProperty("OnUpdate", Variant.Type.String, UseOnUpdate ? PropertyUsageFlags.Default : PropertyUsageFlags.NoEditor, "On Update");
        addProperty("OnPhysicsUpdate", Variant.Type.String, UseOnPhysicsUpdate ? PropertyUsageFlags.Default : PropertyUsageFlags.NoEditor, "On Physics Update");
        return _properties;
    }

    private void addProperty(string varName, Variant.Type type, PropertyUsageFlags usage, string ui_hint)
    {
        _properties.Add(new Godot.Collections.Dictionary()
        {
            { "name", varName },
            { "type", (int)type },
            { "usage", (int)usage }, // See above assignment.
            { "hint", (int)PropertyHint.Expression },
            { "hint_string", ui_hint }
        });
    }

    public override void _Ready()
    {
        if(GetParent() is StateMachine)
            Machine = Machine ?? GetParent() as StateMachine;
    }

    public virtual void Enter() { if (UseOnEnter) processExpression(OnEnter); return; }
    public virtual void Exit() { if (UseOnExit) processExpression(OnExit); return; }
    public virtual void Update(float delta) { if (UseOnUpdate) processExpression(OnUpdate); return; }
    public virtual void PhysicsUpdate(float delta) { if (UseOnPhysicsUpdate) processExpression(OnPhysicsUpdate); return; }

    public void Transition(string state)
    {
        bool state_exists = false;
        foreach (State check_state in TransitionStates)
        {
            if (check_state.Name == state) {
                state_exists = true; break;
            }
        }
        if (!state_exists) return;
        Machine.OnChildTransition(this, state);
    }

    public void Transition(State state = null)
    {
        if (ExitStates.Length == 0 && state == null) return;
        State targetState;
        if (state == null)
        {
            if (ExitStates.Length == 1)
            {
                targetState = ExitStates[0];
            }
            else
            {
                int rand_ind = Random.Shared.Next(0, ExitStates.Length - 1);
                targetState = ExitStates[rand_ind];
            }
        }
        else
            targetState = state;
        Machine.OnChildTransition(this, targetState.Name);
    }

    private Variant processExpression(string expression)
    {
        if (expression == null || expression == "") return 0;
        Expression exp = new Expression();
        Error err = exp.Parse(expression);
        if (err == Error.Ok)
        {
            Variant result = exp.Execute(baseInstance: this);
            if (!exp.HasExecuteFailed())
                return result;
            else
            {
                GD.PrintErr("Expression '" + expression + "' not valid.");
                return 0;
            }
        }
        else
        {
            GD.PrintErr(err);
            return 0;
        }
    }
}
