using Godot;
using System;

public partial class MainMenu : CanvasLayer
{
    [Export] public Control Focus;
    [Export] public Control CreditsFocus;
    [Export] public Control Credits;
	[Export] public PackedScene Game;

	private bool credits = false;

	public override void _Ready()
	{
		Focus.GrabFocus();
	}

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        if (Credits != null)
        {
            if (credits)
                Credits.Modulate = Credits.Modulate.Lerp(new Color(1f, 1f, 1f, 1f), (float)delta * 5f);
            else
                Credits.Modulate = Credits.Modulate.Lerp(new Color(1f, 1f, 1f, 0f), (float)delta * 5f);
        }
    }

    public void StartGame()
    {
        CarryData.Instance.PlayIntro = true;
        GetTree().ChangeSceneToPacked(Game);
    }

    public void CreditsOpen()
    {
        credits = true;
        CreditsFocus.GrabFocus();
    }

    public void CreditsClose()
    {
        credits = false;
        Focus.GrabFocus();
    }

    public void QuitGame()
    {
        GetTree().Quit();
    }
}
