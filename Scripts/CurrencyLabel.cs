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
    Vector2 scaleUp = new Vector2(1.75f, 1.75f);
    Vector2 vectorChanging = new Vector2();
    Color green = new Color("5dff9c");

    public UIButtonsManager uIButtonsManager;
    public void UpdateLabel(float currency)
    {
        elapsedLerpTime = 0;
        currencyToGo = currency;
        originalCurrency = Convert.ToInt32(Text);
        if (originalCurrency < currencyToGo)
        {
            SelfModulate = green;
        }

        
        changingCurrency = true;
        RectScale = scaleUp;
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
                vectorChanging.x = vectorChanging.y = Mathf.Lerp(scaleUp.y, 1, elapsedLerpTime);
                RectScale = vectorChanging;
                Text = Convert.ToInt32(newCurrency).ToString();
                if (elapsedLerpTime > .9f)
                {
                    SelfModulate = Colors.White;
                }
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
