using Godot;
using Godot.Collections;
using System;

public partial class Grab : Area3D
{
    [ExportGroup("Item Details")]
    [Export] public ThiefController controller;
    [Export] public Node3D Hand;
    [Export] public Marker3D Grip;
    [Export] public string RelicGroup = "Relic";
    [ExportGroup("Throwing")]
    [Export] public float ThrowStrength = 0f;
    [Export] public float ThrowAngle = 45f;


    public Node3D Item;

    //public override void _Process(double delta)
    //{
    //    if(Item != null)
    //        Item.Scale = Vector3.One / Hand.Scale;
    //}

    public void OnHit(Node3D body)
    {
        //GD.Print("Grab");
        if (body.IsInGroup(RelicGroup) && Item == null)
        {
            grabItem(body);
        }
    }

    private void grabItem(Node3D item)
    {
        //GD.Print("Grab "+item.Name);
        Item = item;
        Item.Reparent(Grip);

        Item.Position = Vector3.Zero;
        Item.RotationDegrees = Vector3.Zero;
        Item.Scale = Vector3.One / Hand.Scale;

        Relic relic = Item as Relic;
        relic.IsGrabbed = true;
        relic.Velocity = Vector3.Zero;
    }

    public void DropItem()
    {
        if (Item != null)
        {
            if (IsInstanceValid(Item))
            {
                Relic relic = Item as Relic;
                relic.IsGrabbed = false;
                relic.SetVelocity(Vector3.Zero);
                relic.Reparent(GetTree().Root);

                if (ThrowStrength > 0f)
                {
                    Vector3 forward = (controller.Basis * Vector3.Back).Normalized();
                    Vector3 right = (controller.Basis * Vector3.Right).Normalized();
                    float angle = Mathf.DegToRad(ThrowAngle);

                    Vector3 throwVector = forward.Rotated(right, angle).Normalized() * ThrowStrength;
                    relic.SetVelocity(throwVector + controller.Velocity);
                }
                else relic.SetVelocity(controller.Velocity);

                //GD.Print("Item is used: "+ relic.Used);
                if (controller.loot.Open && !relic.Used)
                {
                    controller.loot.AddLoot(relic);
                    Loot selected = controller.loot.SelectedLoot;
                    CarryData.Instance.RemainingLootValue -= selected.GetValue();
                    if (selected != null)
                        controller.UI.UpdateSelectedItem(
                            selected?.name ?? "",
                            controller.loot.SelectedCount,
                            selected.magic?.name ?? "",
                            selected.UI ?? null
                        );
                    else
                        controller.UI.UpdateSelectedItem("", 0, "", null);
                }
            }

            Item = null;
        }
    }

    public void ActivateItem()
    {
        //GD.Print("Activate");
        if (Item != null)
        {
            Relic relic = Item as Relic;
            if (!relic.Used && relic.effect != null)
            {
                relic.IsUsed();
                ItemUsed(relic);
                relic.effect.Activate();
            }
        }
    }

    public void ItemUsed(Relic relic)
    {
        Timer timer = new Timer();
        timer.Connect("timeout", new Callable(this, "clearItem"));
        timer.WaitTime = relic.UsedTimeout;
        timer.Autostart = true;
        timer.OneShot = true;
        this.AddChild(timer);
    }

    private void clearItem()
    {
        if (Item != null)
        {
            Relic relic = Item as Relic;
            if (relic.Used)
                Item = null;
        }
    }

    public void GetItem()
    {
        //GD.Print("Get Item");
        if (Item == null && controller.loot.Open)
        {
            Node3D item = controller.loot.RemoveLoot();
            if (item != null)
            {
                Item = item;
                grabItem(item);
                Loot selected = controller.loot.SelectedLoot;
                CarryData.Instance.RemainingLootValue += selected.GetValue();
                if (selected != null)
                    controller.UI.UpdateSelectedItem(
                        selected?.name ?? "",
                        controller.loot.SelectedCount,
                        selected.magic?.name ?? "",
                        selected.UI ?? null
                    );
                else
                    controller.UI.UpdateSelectedItem("", 0, "", null);
            }
        }
    }
}
