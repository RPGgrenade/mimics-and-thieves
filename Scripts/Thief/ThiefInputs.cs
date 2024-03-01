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
        Run = InputActionBuffer.Instance.IsActionBuffered("Run");
        Dodge = InputActionBuffer.Instance.ConsumeAction("Dodge");
        Grab = InputActionBuffer.Instance.IsActionBuffered("Grab");
        Bag = InputActionBuffer.Instance.IsActionBuffered("Bag");
        Swing = InputActionBuffer.Instance.ConsumeAction("Swing");

        // UI input grabbing
        Pause = Input.IsActionJustPressed("Pause");

        if(Input.IsActionJustReleased("CycleLeft") || Input.IsActionJustReleased("CycleRight"))

        CycleLeft = Input.IsActionPressed("CycleLeft") && CycleCooldown <= 0f;
        CycleRight = Input.IsActionPressed("CycleRight") && CycleCooldown <= 0f;

        if ((CycleLeft || CycleRight)) cycleCooldown = CycleCooldown;
    }
}
