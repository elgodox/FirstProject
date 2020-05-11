using Godot;
using System;

public class CurrencyLabel : Label
{
    [Export] public string myType;
    [Export] public bool bonusPrice;

    float elapsedLerpTime;
    float newCurrency;
    float currencyToGo;
    float originalCurrency;
    float speedOfChange = 3;
    bool changingCurrency;

    public UIButtonsManager uIButtonsManager;
    public void UpdateLabel(float currency)
    {
        elapsedLerpTime = 0;
        currencyToGo = currency;
        originalCurrency = Convert.ToInt32(Text);
        changingCurrency = true;
    }
    public override void _Ready()
    {
        uIButtonsManager = GetTree().Root.GetNode("GameManager/UI_Template") as UIButtonsManager;
        if(!bonusPrice)
            SuscribeMe();
    }

    public override void _Process(float delta)
    {
        if (changingCurrency)
        {
            if (elapsedLerpTime <= 1)
            {
                elapsedLerpTime += delta * speedOfChange;
                elapsedLerpTime = Mathf.Clamp(elapsedLerpTime, 0, 1);
                newCurrency = Mathf.Lerp(originalCurrency, currencyToGo, elapsedLerpTime);
                
                Text = Convert.ToInt32(newCurrency).ToString();
            }
            else
            {
                elapsedLerpTime = 1;
                changingCurrency = false;
            }
        }
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
