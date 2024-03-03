using Godot;
using System;

public partial class ThiefInputs : Node
{
    [ExportCategory("Axis Inputs")]
    [Export] public Vector2 LeftStick = new Vector2(0f, 0f);
    [Export] public Vector2 RightStick = new Vector2(0f, 0f);
    [Export] public float StickSmoothness = 20f;
    [Export] public Vector2 MouseInput = new Vector2(0f, 0f);
    [Export] public float MouseSmoothness = 20f;

    [ExportCategory("Button Inputs")]
    [ExportGroup("Actions")]
    [Export] public bool Jump = false;
    [Export] public bool Run = false;
    [Export] public bool Dodge = false;
    [Export] public bool Grab = false;
    [Export] public bool Bag = false;
    [Export] public bool Swing = false;
    [ExportGroup("Inventory")]
    [Export] public bool Pause = false;
    [Export] public bool CycleLeft = false;
    [Export] public bool CycleRight = false;
    [Export] public float CycleCooldown = 0.25f;

    [ExportGroup("Utility")]
    [Export] public bool JustGrabbed = false;
    [Export] public bool JustDropped = false;


    private float cycleCooldown = 0.0f;

    public override void _Process(double delta)
    {
        // Stick input grabbing
        LeftStick.X = Input.GetAxis("Left", "Right");
        LeftStick.Y = Input.GetAxis("Up", "Down");
        MouseInput = MouseInput.Lerp(Input.GetLastMouseVelocity(), (float)delta * MouseSmoothness);
        RightStick = RightStick.Lerp(
            new Vector2(
                Input.GetAxis("Left2", "Right2"),
                Input.GetAxis("Up2", "Down2")
            ),
        (float)delta * StickSmoothness);

        // Action input grabbing
        // Consume action check is for things that stop after being processed
        // Is buffered is for held things
        Jump = InputActionBuffer.Instance.ConsumeAction("Jump");
        Run = Input.IsActionPressed("Run");
        Dodge = InputActionBuffer.Instance.ConsumeAction("Dodge");
        Grab = Input.IsActionPressed("Grab");
        JustGrabbed = Input.IsActionJustPressed("Grab");
        JustDropped = Input.IsActionJustReleased("Grab");
        Bag = Input.IsActionPressed("Bag");
        Swing = InputActionBuffer.Instance.ConsumeAction("Swing");

        // UI input grabbing
        Pause = Input.IsActionJustPressed("Pause");

        if (cycleCooldown > 0f) cycleCooldown -= (float)delta;
        if(Input.IsActionJustReleased("CycleLeft") || Input.IsActionJustReleased("CycleRight")) cycleCooldown = 0f;

        CycleLeft = Input.IsActionPressed("CycleLeft") && cycleCooldown <= 0f;
        CycleRight = Input.IsActionPressed("CycleRight") && cycleCooldown <= 0f;

        if ((CycleLeft || CycleRight)) cycleCooldown = CycleCooldown;
    }
}
