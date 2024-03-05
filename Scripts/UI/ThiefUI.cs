using Godot;
using Godot.Collections;
using System;
using System.Linq;
using System.Reflection;
using static Inventory;

public partial class ThiefUI : CanvasLayer
{
    [Export] public bool Paused = false;
    [Export] public float PauseSpeed = 10f;
    [Export] public float OpacityThreshold = 0.1f;

    [ExportCategory("Defaults")]
    [Export] public string DefaultName = "";
    [Export] public int DefaultCount = 0;
    [Export] public string DefaultEffect = "";
    [Export] public Texture2D DefaultTexture = null;

    [ExportCategory("Selected Item")]
    [Export] public Control SelectedControl;
    [Export] public Label ItemName;
    [Export] public TextureRect ItemTexture;
    [Export] public Label ItemCount;
    [Export] public Label ItemEffect;

    [ExportCategory("Inventory")]
    [Export] public Control InventoryControl;
    [Export] public InventoryItem[] Inventory;
    [ExportGroup("Info Panel")]
    [Export] public TextureRect InfoTexture;
    [Export] public Label InfoName;
    [Export] public Label InfoDescription;
    [Export] public Label InfoEffect;
    [Export] public Label InfoEffectDescription;

    private Dictionary<string, InventorySlot> _allLoot = new Dictionary<string, InventorySlot>();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        ItemName.Text = DefaultName;
        ItemCount.Text = ("" + DefaultCount);
        ItemEffect.Text = DefaultEffect;
        ItemTexture.Texture = DefaultTexture;

        SelectedControl.Modulate = new Color(1f, 1f, 1f, 1f);
        InventoryControl.Modulate = new Color(0f, 0f, 0f, 0f);

        Inventory[0].GrabFocus();

        UpdateInventory();
    }

	public override void _Process(double delta)
    {
        SelectedControl.Modulate = SelectedControl.Modulate.Lerp(new Color(1f, 1f, 1f, Paused ? 0f : 1f), (float)delta * PauseSpeed);
        InventoryControl.Modulate = InventoryControl.Modulate.Lerp(new Color(1f, 1f, 1f, Paused ? 1f : 0f), (float)delta * PauseSpeed);

        InventoryControl.ProcessMode = (InventoryControl.Modulate.A < OpacityThreshold) ? ProcessModeEnum.Disabled : ProcessModeEnum.Inherit;
        SelectedControl.ProcessMode = (SelectedControl.Modulate.A < OpacityThreshold) ? ProcessModeEnum.Disabled : ProcessModeEnum.Inherit;

        CheckInfo();
    }

    public void CheckInfo()
    {
        int focus_index = 0;
        for (int i = 0; i < Inventory.Length; i++)
        {
            if (Inventory[i].HasFocus()) {
                focus_index = i;
                break;
            }
        }
        if(_allLoot != null && _allLoot.Keys.Count > focus_index)
            UpdateInfo(_allLoot[_allLoot.Keys.ToList()[focus_index]].loot);
        else
            UpdateInfo(null);
    }

    public void UpdateInfo(Loot loot = null)
    {
        if (loot == null)
        {
            InfoName.Text = DefaultName;
            InfoTexture.Texture = DefaultTexture;
            InfoDescription.Text = DefaultName;
            InfoEffect.Text = DefaultName;
            InfoEffectDescription.Text = DefaultName;
        }
        else
        {
            InfoName.Text = loot.name;
            InfoTexture.Texture = loot.UI;
            InfoDescription.Text = loot.description;
            InfoEffect.Text = loot.magic != null ? loot.magic.name : DefaultEffect;
            InfoEffectDescription.Text = loot.magic != null ? loot.magic.Description : DefaultEffect;
        }
    }

    public void UpdateInventory(Dictionary<string, Inventory.InventorySlot> inventory = null)
    {
        foreach(InventoryItem item in Inventory)
        {
            GD.Print(item.Name);
            item.ItemTexture.Texture = DefaultTexture;
            item.ItemCount.Text = ""+DefaultCount;
        }
        if (inventory != null)
        {
            int index = 0;
            var keys = inventory.Keys.ToList();
            foreach (var key in keys)
            {
                Inventory.InventorySlot slot = inventory[key];
                Inventory[index].ItemTexture.Texture = slot.loot.UI;
                Inventory[index].ItemCount.Text = "" + slot.count;
                index++;

                if (Inventory[index].HasFocus())
                {
                    UpdateInfo(slot.loot);
                }
            }
        }
        _allLoot = inventory;
    }

    public void UpdateSelectedItem(string name = "", int count = 0, string effect = "", Texture2D texture = null)
    {
        ItemName.Text = name;
        ItemCount.Text = ""+count;
        ItemEffect.Text = effect;
        ItemTexture.Texture = texture ?? DefaultTexture;
    }
}
