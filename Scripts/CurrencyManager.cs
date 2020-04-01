using Godot;
using System;

public class CurrencyManager : Node
{
    
	[Signal] public delegate void CurrencyChanged(string typeOfCurrency, float currency);
	[Signal] public delegate void GameHaveBet(bool haveBet);

    [Export] float credit, maxBetAmount, minBetAmount;
    float currentBet, currencyToCollect;
    public override void _Ready()
    {
        CheckAllCurrency();
    }
    void Bet() // La llama UIManager, señal bet
    {
        if(credit >= minBetAmount)
        {
            if(currentBet >= maxBetAmount || currentBet + minBetAmount > credit)
            {
                currentBet = minBetAmount;
                EmitSignal(nameof(GameHaveBet), false);
            }
            else
            {
                currentBet += minBetAmount;
                EmitSignal(nameof(GameHaveBet), true);
            }
        }
        EmitSignal(nameof(CurrencyChanged), Constants.currentBet, currentBet);
    }
    void MaxBet() // La llama UIManager, señal MaxBet
    {
        if(credit >= maxBetAmount)
        {
            currentBet = maxBetAmount;
            EmitSignal(nameof(GameHaveBet), true);
            EmitSignal(nameof(CurrencyChanged), Constants.currentBet, currentBet);
        }
        
    }
    void Collect() // La llama UIManager, señal collect
    {
        GD.Print("Collect");
    }
    void ConfirmBet() // La llama UIManager, señal restartGame
    {
        credit -= currentBet;
        EmitSignal(nameof(CurrencyChanged), Constants.credits, credit);
    }

    void AddBetToCurrency() // La lama GameManager, señal roundWinned
    {
        currencyToCollect += currentBet;
        EmitSignal(nameof(CurrencyChanged), Constants.currencyToCollect, currencyToCollect);
    }
    void WinnedCurrency()
    {
        EmitSignal(nameof(CurrencyChanged), Constants.winnedCurrency, currencyToCollect);
    }
    void AddCurrencyToCredits(bool win) // La llama GameManager, señal GameOver
    {
        if(win)
        {
            WinnedCurrency();
            credit += currencyToCollect;
        }

        currencyToCollect = 0;

        EmitSignal(nameof(CurrencyChanged), Constants.currencyToCollect, currencyToCollect);
        EmitSignal(nameof(CurrencyChanged), Constants.credits, credit);

        CheckCreditsToKeepPlaying();
    }
    void CheckCreditsToKeepPlaying() 
    {
        if(credit >= currentBet)
        {
            EmitSignal(nameof(GameHaveBet), true);
        }
        else
            EmitSignal(nameof(GameHaveBet), false);
    }
    void CheckAllCurrency()
    {
        EmitSignal(nameof(CurrencyChanged), Constants.credits, credit);
        EmitSignal(nameof(CurrencyChanged), Constants.currencyToCollect, currencyToCollect);
        EmitSignal(nameof(CurrencyChanged), Constants.currentBet, currentBet);
    }
}
