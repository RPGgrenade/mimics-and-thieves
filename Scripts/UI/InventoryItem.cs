using Godot;
using System;

public partial class InventoryItem : PanelContainer
{
    [Export] public StyleBoxTexture NoHighlight;
    [Export] public StyleBoxTexture Highlight; 
	[ExportCategory("Item")]
	[Export] public TextureRect ItemTexture;
	[Export] public Label ItemCount;

    public void SetNoHighlight()
    {
        AddThemeStyleboxOverride("panel", NoHighlight);
    }
    public void SetHighlight()
    {
        AddThemeStyleboxOverride("panel", Highlight);
    }
}
