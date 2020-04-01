using Godot;
using System;

public class CurrencyLabel : Label
{
    [Export] public string myType;

    public UIButtonsManager uIButtonsManager;
    public void UpdateLabel(string newLabel)
    {
        Text = newLabel;
    }
    public override void _Ready()
    {
        uIButtonsManager = GetTree().Root.GetNode("GameManager/UI_Template") as UIButtonsManager; 
        if(uIButtonsManager != null)
            uIButtonsManager.SubscribeCurrencyLabel(this);
    }
}
