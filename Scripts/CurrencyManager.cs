using Godot;
using System;

public class CurrencyManager : Node
{
    
	[Signal] public delegate void CurrencyChanged(params object[] parameters);
	[Signal] public delegate void GameHaveBet(bool haveBet);
    double[] multipliers = new double[10];

    public double credit, maxBetAmount, minBetAmount;
    public double currentBet, currencyToCollect;

    double multiplier;
    public override void _Ready()
    {
        multipliers[0]= 0;
        multipliers[1]= 0.9;
        multipliers[2]= 1.2;
        multipliers[3]= 1.4;
        multipliers[4]= 1.68;
        multipliers[5]= 2.1;
        multipliers[6]= 2.8;
        multipliers[7]= 4.2;
        multipliers[8]= 8.4;
        multipliers[9]= 16.8;
    }

    void SetCurrency(double money, double minBet,double maxBet)
    {
        credit = money;
        minBetAmount = minBet;
        maxBetAmount = maxBet;
        CheckAllCurrency();
        CheckBet();
    }
    void Bet() // La llama UIManager, señal bet
    {
        if(credit >= minBetAmount)
        {
            if(currentBet >= maxBetAmount || currentBet + minBetAmount > credit)
            {
                currentBet = 0;
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
        if(credit > 0)
        {
            if(credit >= maxBetAmount) { currentBet = maxBetAmount; }
            else { currentBet = credit; }
            
            EmitSignal(nameof(GameHaveBet), true);
            EmitSignal(nameof(CurrencyChanged), Constants.currentBet, currentBet);
        }
    }
    void Collect() // La llama UIManager, señal collect
    {
        credit = 0;
        currentBet = 0;
        EmitSignal(nameof(CurrencyChanged), Constants.credits, credit);
        EmitSignal(nameof(CurrencyChanged), Constants.currentBet, currentBet);
        
        CheckBet();
    }
    void ConfirmBet() // La llama UIManager, señal restartGame
    {
        credit -= currentBet;
        EmitSignal(nameof(CurrencyChanged), Constants.credits, credit);
    }
    void AddBetToCurrency() // La llama GameManager, señal roundWinned
    {
        currencyToCollect = currentBet * multiplier;
        EmitSignal(nameof(CurrencyChanged), Constants.currencyToCollect, currencyToCollect);
    }
    void UpdateWinnedCurrency()
    {
        EmitSignal(nameof(CurrencyChanged), Constants.winnedCurrency, currencyToCollect);
    }
    void AddCurrencyToCredits(bool win) // La llama GameManager, señal GameOver
    {
        if(win)
        {
            UpdateWinnedCurrency();
            credit += currencyToCollect;
        }

        currencyToCollect = 0;

        EmitSignal(nameof(CurrencyChanged), Constants.currencyToCollect, currencyToCollect);
        EmitSignal(nameof(CurrencyChanged), Constants.credits, credit);

        CheckBet();
    }
    
    void CheckAllCurrency()
    {
        EmitSignal(nameof(CurrencyChanged), Constants.credits, credit);
        EmitSignal(nameof(CurrencyChanged), Constants.currencyToCollect, currencyToCollect);
        EmitSignal(nameof(CurrencyChanged), Constants.currentBet, currentBet);
    }

    public void SetMultiplier(int levelMultiplier)
    {
        if(levelMultiplier < 0) // Checkeo que si al enviar un -1 no intente acceder al indice [-1] del array
        {
            levelMultiplier = 0;
        }
        multiplier = multipliers[levelMultiplier];
    }

    void CheckBet() 
    {
        if(credit > 0)
        {
            if(currentBet > 0)
            {
                EmitSignal(nameof(GameHaveBet), true);
            }
        }
        
        else
        {
            currentBet = 0;
            EmitSignal(nameof(GameHaveBet), false);
        }
    }
}
