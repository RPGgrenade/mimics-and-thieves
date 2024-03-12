using Godot;
using System;
using System.Linq;

public partial class IntrouUI : CanvasLayer
{
	[Export] public Control Focus;
	[Export] public Control Main;
	[Export] public Control[] Texts;
	[Export] public bool Opaque = true;
    [Export] public float TextSpeed = 7f;
    [Export] public float IntroSpeed = 5f;

    private Control currentText;
	private int index = -1;

	public override void _Ready()
	{
		if(!CarryData.Instance.PlayIntro)
			QueueFree(); // Just kill it
		Focus.GrabFocus();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Opaque)
			Main.Modulate = Main.Modulate.Lerp(new Color(1f, 1f, 1f, 1f), (float)delta * IntroSpeed);
        else
            Main.Modulate = Main.Modulate.Lerp(new Color(1f, 1f, 1f, 0f), (float)delta * IntroSpeed);

		GD.Print("Modulate: "+ Main.Modulate);
		if (Main.Modulate.A <= 0.04f)
		{
			CarryData.Instance.PlayIntro = false;
            QueueFree();
		}

        foreach (Control text in Texts)
        {
            if (text == currentText)
                text.Modulate = text.Modulate.Lerp(new Color(1f, 1f, 1f, 1f), (float)delta * TextSpeed);
            else
                text.Modulate = text.Modulate.Lerp(new Color(1f, 1f, 1f, 0f), (float)delta * TextSpeed);
        }
	}

	public void AdvanceText()
	{
		index++;
		if (index < Texts.Count())
		{
			currentText = Texts[index];
			GD.Print("Index is "+index);
		}
		else
        {
            GD.Print("No more index");
            currentText = null;
			Opaque = false;
		}
	}
}
