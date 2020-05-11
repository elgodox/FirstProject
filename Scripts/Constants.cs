using Godot;
using System;

public class Constants
{
    #region Paths

    public const string PATH_UI_GAMEOVR_WIN = "res://Prefabs/UI/UI_GameOver(WIN).tscn";
    public const string PATH_UI_GAMEOVR_LOSE = "res://Prefabs/UI/UI_GameOver.tscn";
    public const string PATH_BACKGROUND = "res://Prefabs/BackgroundCity.tscn";
    public const string PATH_BONUS = "res://Prefabs/BonusManager.tscn";
    public const string PATH_LEVEL_MANAGER = "res://Prefabs/LevelManager.tscn";
        
    #endregion

    #region Keys
    public const string CURRENT_BET = "currentBet";
    public const string CREDITS = "credits";
    public const string CURRENCY_TO_COLLECT = "currencyToCollect";
    public const string CURRENCY_TO_COLLECT_NEXT = "currencyToCollectNextLevel";
    public const string CURRENCY_WINNED = "winnedCurrency";
    public const string CURRENCY_WINNED_IN_BONUS = "winnedCurrencyInBonus";
    public const string CURRENCY_BONUS_REWARD = "bonusReward";
    public const string CURRENCY_TOTAL_BONUS_REWARD = "totalBonusReward";
        
    #endregion


    public static readonly double[] BONUS_MULTIPLIERS = new double[] { 1, 1.5d, 2, 3};
}
