using Godot;
using Godot.Collections;

[GodotClassName("InputActionBuffer")]
public partial class InputActionBuffer : Node
{
    public static InputActionBuffer Instance;
    [Export]
    public ulong BufferTimeMs = 150;
    private Dictionary<string, ulong> _actionPressTimes = new();
    private Dictionary<string, ulong> _customBufferTimes = new();

    public override void _Ready()
    {
        Instance = this;
    }

    // Allows the action with the given name to be queried every frame.
    // This method is called automatically by IsActionBuffered() or ConsumeAction(),
    // but it could be useful to call this method early
    // to start buffering actions before they're queried for the first time.
    //
    // For example, if a "jump" action is pressed just before the player
    // gains control of their character for the first time,
    // that first input will be missed if RegisterAction("jump") wasn't called previously.
    //
    // Takes an optional bufferTimeMs parameter. If included, sets this specific action's buffer window, in milliseconds.
    // Otherwise, the buffer's default time is used.
    public void RegisterAction(string action, ulong? bufferTimeMs = null)
    {
        if (!_actionPressTimes.ContainsKey(action))
        {
            _actionPressTimes[action] = 0;
        }
        if (bufferTimeMs.HasValue)
        {
            _customBufferTimes[action] = bufferTimeMs.Value;
        }
    }

    // Returns whether the given action has been pressed recently,
    // within the buffer window and is ready to be performed,
    // but without consuming it.
    // See ConsumeAction().
    public bool IsActionBuffered(string action)
    {
        if (!_actionPressTimes.ContainsKey(action))
        {
            RegisterAction(action);
            return false;
        }
        ulong now = Time.GetTicksMsec();
        ulong lastPressTime = _actionPressTimes[action];
        ulong msSincePress = now - lastPressTime;
        ulong bufferTime = _customBufferTimes.TryGetValue(action, out ulong time) ? time : BufferTimeMs;
        
        return lastPressTime != 0 && msSincePress <= bufferTime;
    }

    // Checks whether the given action has been pressed recently and is ready to be performed.
    // If that is the case, the action is "consumed" and true is returned.
    // If the action hasn't been pressed recently, returns false.
    //
    // "Consuming" the action prevents this input from triggering any more actions
    // before the button is released and pressed again.
    public bool ConsumeAction(string action)
    {
        if (IsActionBuffered(action))
        {
            // Consume
            _actionPressTimes[action] = 0;
            return true;
        }

        return false;
    }

    public override void _Process(double delta)
    {
        ulong now = Time.GetTicksMsec();
        foreach (string action in _actionPressTimes.Keys)
        {
            if (Input.IsActionJustPressed(action))
            {
                _actionPressTimes[action] = now;
            }
        }
    }
}