using Godot;
using System;

public class CurrencyLabel : Label
{
    [Export] public string myType;
    [Export] public bool bonusPrice;

    public UIButtonsManager uIButtonsManager;
    public void UpdateLabel(string newLabel)
    {
        Text = newLabel;
    }
    public override void _Ready()
    {
        uIButtonsManager = GetTree().Root.GetNode("GameManager/UI_Template") as UIButtonsManager;
        if(!bonusPrice)
            SuscribeMe();
    }

    public void UpdateMyType(string myNewType)
    {
        myType = myNewType;
        SuscribeMe();
    }
    public void UnsuscribeMe() //La llama BonusManager, señal BonusPicked
    {
        uIButtonsManager.UnsuscribeCurrencyLabel(this);
    }

    public void SuscribeMe()
    {
        if(uIButtonsManager != null)
            uIButtonsManager.SubscribeCurrencyLabel(this);
    }
}
