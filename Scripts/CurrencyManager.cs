using Godot;
using System;

public class CurrencyManager : Node
{
    
	[Signal] public delegate void CurrencyChanged(params object[] parameters);
	[Signal] public delegate void GameHaveBet(bool haveBet);
    double[] multipliers = { 0, 0.9, 1.2, 1.4, 1.68, 2.1, 2.8, 4.2, 8.4, 16.8 };
    public double credit, maxBetAmount, minBetAmount;
    public double currentBet, currencyToCollect;

    double multiplier;

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
        EmitSignal(nameof(CurrencyChanged), Constants.CURRENT_BET, currentBet);
    }
    void MaxBet() // La llama UIManager, señal MaxBet
    {
        if(credit > 0)
        {
            if(credit >= maxBetAmount) { currentBet = maxBetAmount; }
            else { currentBet = credit; }
            
            EmitSignal(nameof(GameHaveBet), true);
            EmitSignal(nameof(CurrencyChanged), Constants.CURRENT_BET, currentBet);
        }
    }
    /*void Collect() // La llama UIManager, señal collect
    {
        AddCurrencyToCredits(true);
    }*/

   /* void CollectOmenu() // La llama UIManager, señal collect
    {
        credit = 0;
        currentBet = 0;
        
        CheckBet();
    }*/
    void ConfirmBet() // La llama UIManager, señal restartGame
    {
        credit -= currentBet;
        EmitSignal(nameof(CurrencyChanged), Constants.CREDITS, credit);
    }
    void AddBetToCurrency() // La llama GameManager, señal roundWinned
    {
        currencyToCollect = currentBet * multiplier;
        EmitSignal(nameof(CurrencyChanged), Constants.CURRENCY_TO_COLLECT, currencyToCollect);
    }
    void UpdateWinnedCurrency()
    {
        EmitSignal(nameof(CurrencyChanged), Constants.CURRENCY_WINNED, currencyToCollect);
    }

    void UpdateBonusPrices() //La llama GameManager, señal BonusStarted
    {
        EmitSignal(nameof(CurrencyChanged), Constants.BONUS_MULTIPLIERS[0], Constants.BONUS_MULTIPLIERS[0] * currentBet);
        EmitSignal(nameof(CurrencyChanged), Constants.BONUS_MULTIPLIERS[1], Constants.BONUS_MULTIPLIERS[1] * currentBet);
        EmitSignal(nameof(CurrencyChanged), Constants.BONUS_MULTIPLIERS[2], Constants.BONUS_MULTIPLIERS[2] * currentBet);
        EmitSignal(nameof(CurrencyChanged), Constants.BONUS_MULTIPLIERS[3], Constants.BONUS_MULTIPLIERS[3] * currentBet);
    }

    public void UpdateBonusReward(double multiplier)
    {
        EmitSignal(nameof(CurrencyChanged), Constants.CURRENCY_BONUS_REWARD, currentBet * multiplier);
    }

    public void ResetCurrencyToCollect()
    {
        currencyToCollect = 0;
        EmitSignal(nameof(CurrencyChanged), Constants.CURRENCY_TO_COLLECT, currencyToCollect);
    }
    void AddCurrencyToCredits() // La llama GameManager, señal GameOver
    {
        UpdateWinnedCurrency();
        credit += currencyToCollect;
        
        currencyToCollect = 0;
        currentBet = 0;
        
        EmitSignal(nameof(CurrencyChanged), Constants.CURRENT_BET, currentBet);
        EmitSignal(nameof(CurrencyChanged), Constants.CURRENCY_TO_COLLECT, currencyToCollect);
        EmitSignal(nameof(CurrencyChanged), Constants.CREDITS, credit);

        CheckBet();
    }
    
    void CheckAllCurrency()
    {
        EmitSignal(nameof(CurrencyChanged), Constants.CREDITS, credit);
        EmitSignal(nameof(CurrencyChanged), Constants.CURRENCY_TO_COLLECT, currencyToCollect);
        EmitSignal(nameof(CurrencyChanged), Constants.CURRENT_BET, currentBet);
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
