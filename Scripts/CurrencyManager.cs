using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

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
        //GD.Print("Set Currency recibe creditos: " + money);
        credit = money;
        minBetAmount = minBet;
        maxBetAmount = maxBet;
        CheckAllCurrency();
        //CheckBet();
    }
    public void BetUp() // La llama UIManager, señal betUp
    {
        if (credit <= 0)
        {
            EmitSignal(nameof(GameHaveBet), false);
            return;
        }
        if(credit >= minBetAmount)
        {
            
            if(currentBet >= maxBetAmount || currentBet >= credit)
            {
                currentBet = minBetAmount;
            }
            else if (currentBet + minBetAmount > credit)
            {
                currentBet = credit;
            }
            else
            {
                currentBet += minBetAmount;
                if (currentBet > maxBetAmount) { currentBet = maxBetAmount;}
            }
        }
        else
        {
            currentBet = credit;
        }
        EmitSignal(nameof(GameHaveBet), true);
        EmitSignal(nameof(CurrencyChanged), Constants.CURRENT_BET, currentBet);
    }

    void BetDown() // La llama UIManager, señal betDown
    {
        if (credit <= 0)
        {
            EmitSignal(nameof(GameHaveBet), false);
            return;
        }
        if(credit >= minBetAmount)
        {
            if(currentBet <= minBetAmount)
            {
                if (credit >= maxBetAmount)
                {
                    currentBet = maxBetAmount;
                }
                else
                {
                    currentBet = credit;
                }
            }
            else
            {
                currentBet -= minBetAmount;
                if (currentBet < minBetAmount) { currentBet = minBetAmount;}
            }
        }
        else
        {
            currentBet = credit;
        }
        EmitSignal(nameof(GameHaveBet), true);
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
    public void ConfirmBet() // La llama UIManager, señal restartGame
    {
        credit -= currentBet;
        EmitSignal(nameof(CurrencyChanged), Constants.CREDITS, credit);
    }
    public void AddBetToCurrency() // La llama GameManager, señal roundWinned
    {
        currencyToCollect = currentBet * multiplier;
        EmitSignal(nameof(CurrencyChanged), Constants.CURRENCY_TO_COLLECT, currencyToCollect);
        for (int i = 0; i < multipliers.Length; i++)
        {
            if (multipliers[i] == multiplier && i < multipliers.Length - 1)
            {
                EmitSignal(nameof(CurrencyChanged), Constants.CURRENCY_TO_COLLECT_NEXT, currentBet * multipliers[i + 1]);
            }
        }
    }
    public void UpdateWinnedCurrency()
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
        EmitSignal(nameof(CurrencyChanged), Constants.CURRENCY_WINNED_IN_BONUS, currencyToCollect);
        EmitSignal(nameof(CurrencyChanged), Constants.CURRENCY_TOTAL_BONUS_REWARD, currencyToCollect + currentBet * multiplier);

        credit += currentBet * multiplier;
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
        
        EmitSignal(nameof(CurrencyChanged), Constants.CURRENT_BET, currentBet);
        EmitSignal(nameof(CurrencyChanged), Constants.CURRENCY_TO_COLLECT, currencyToCollect);
        EmitSignal(nameof(CurrencyChanged), Constants.CURRENCY_TO_COLLECT_NEXT, currencyToCollect);
        EmitSignal(nameof(CurrencyChanged), Constants.CREDITS, credit);

        CheckBet();
    }
    
    public void CheckAllCurrency()
    {
        EmitSignal(nameof(CurrencyChanged), Constants.CREDITS, credit);
        EmitSignal(nameof(CurrencyChanged), Constants.CURRENCY_TO_COLLECT, currencyToCollect);
        EmitSignal(nameof(CurrencyChanged), Constants.CURRENT_BET, currentBet);
    }

    public void SetLevelMultiplier(int levelMultiplier)
    {
        //GD.Print("Set Multiplier recibe " + levelMultiplier);
        if(levelMultiplier < 0) // Checkeo que si al enviar un -1 no intente acceder al indice [-1] del array
        {
            //GD.Print("Set Multiplier clampea " + levelMultiplier + " a 0");
            levelMultiplier = 0;
        }
        multiplier = multipliers[levelMultiplier];
        //GD.Print("Set Multiplier setea el muliplicador " + multiplier + " que esta en el índice " + levelMultiplier);
    }

    public void CheckBet() 
    {
        GD.Print("Checking bet...");
        if(credit > 0)
        {
            if (currentBet > credit)
            {
                currentBet = credit;
            }

            if (currentBet > 0)
            {
                GD.Print("Hay Bet");
                EmitSignal(nameof(GameHaveBet), true);
            }
            else
            {
                GD.Print("NO HAY bet");
                EmitSignal(nameof(GameHaveBet), false);
            }
            
        }
        
        else
        {
            currentBet = 0;
            GD.Print("NO HAY bet");
            EmitSignal(nameof(GameHaveBet), false);
        }
        EmitSignal(nameof(CurrencyChanged), Constants.CURRENT_BET, currentBet);
    }
}
