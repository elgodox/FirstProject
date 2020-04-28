using Godot;
using System;
using System.Collections;
using System.Collections.Generic;

public class GameManager : Godot.Control
{
    [Signal] public delegate void CanCollect();
    [Signal] public delegate void GameOver(bool win);
    [Signal] public delegate void GameStarted();
    [Signal] public delegate void RoundWined();
    [Signal] public delegate void SetCurrencyManager(double credit, double minBet, double maxBet);
    [Signal] public delegate void GameReady(bool ready);
    [Signal] public delegate void StartTimer(float secs);
    [Signal] public delegate void StartBonus();

    public LevelManager currentLevelMngr;
    int currentLevel;
    string bet_description;
    [Export] bool UseDB = false;
    bool gotBonus = false;
    bool bonusAssigned = false;
    [Export] int levels, timerInLevel;
    [Export] int[] badOnes = new int[10];
    bool isPlaying = false;
    GameRecover myRecover;
    OMenuCommunication oMenu = new OMenuCommunication();
    GameGenerator myGameGen = new GameGenerator();
    CurrencyManager currencyManager;
    int[] currentLevelInfo;
    bool isResuming;

    public override void _Ready()
    {
        currencyManager = GetNode("CurrencyManager") as CurrencyManager;

        if (UseDB)
        {
            oMenu.Start(OnStartCompletation, null);
        }
        else
        {
            EmitSignal(nameof(SetCurrencyManager), 1000, 5, 25);
        }
    }

    #region BBDD Methods
    private void OnStartCompletation()
    {
        oMenu.IsPlaying(delegate { GetBetDescription(delegate (string value) { CheckBetDescription(value); }, null); }, null);
        // Puede llegar a explotar
        oMenu.GetMoney(delegate (double money) { EmitSignal(nameof(SetCurrencyManager), money, oMenu.MinBet(), oMenu.MaxBet()); }, null);
    }
    private void GetBetDescription(Action<string> OnSuccess, Action OnFail)
    {
        oMenu.GetBetDescription(OnSuccess, OnFail);
    }
    void CheckBetDescription(string betToCheck)
    {
        if (betToCheck != null)
        {
            if (betToCheck.Contains("P"))
            {
                bet_description = betToCheck;
                myRecover = new GameRecover(bet_description);
                myRecover.FillDictionarys(levels);

                oMenu.GetMoney(
                    delegate (double money)
                    {
                        currencyManager.credit = money;

                        oMenu.GetCurrentBet(delegate (double bet)
                        {
                            currencyManager.currentBet = bet;
                            ResumeCrashedGame();

                        }, null);

                    }, null);
            }
            else
            {
                PrepareBetDescription();
            }
        }
    }
    void PrepareBetDescription()
    {
        oMenu.GetSaveData(UpdateSaveDataLocal, null);
    }
    void UpdateSaveDataLocal(OMenuClient.Structs.SaveData saveData)
    {
        double money = saveData.moneyAmount;
        double bet = saveData.betAmount;
        double betToMoney = bet + money;
        OMenuClient.Structs.SaveData newSaveData = new OMenuClient.Structs.SaveData(false, betToMoney, 0, DateTime.Now, "");
        oMenu.UpdateSaveData(newSaveData, null, null);
    }
    void UpdateSaveData(String nodePressed)
    {
        if (UseDB)
        {
            if (nodePressed != null)
            {
                bet_description += nodePressed.Replace("Hex", "");
                bet_description += ";";
            }
            else
            {
                bet_description += "|";
            }

            OMenuClient.Structs.SaveData saveData = new OMenuClient.Structs.SaveData(isPlaying, currencyManager.credit, currencyManager.currentBet, DateTime.Now, bet_description);

            oMenu.UpdateSaveData(saveData, null, null);
        }
    }
    private void FillBetDescription(int restOfLevels)
    {
        if (restOfLevels > 0)
        {
            for (int i = 0; i < restOfLevels; i++)
            {
                var intendedLevel = restOfLevels - i;
                myGameGen.SetBadOnes(badOnes[intendedLevel]);
                currentLevelInfo = myGameGen.GenerateLevelInfo(intendedLevel);
                string restOfLevelsDescription = myGameGen.GetLevelDescription(currentLevelInfo);
                bet_description += restOfLevelsDescription;
                UpdateSaveData("|-2");
            }
        }
        GD.Print(bet_description);
    }

    #endregion


    void CreateBonusLevel(string path)
    {
        var newLevelManager = ResourceLoader.Load(path) as PackedScene;
        currentLevelMngr = newLevelManager.Instance() as LevelManager;
        AddChild(currentLevelMngr);
        currentLevelMngr.myGameManager = this;
    }

    void CreateLevel(string path)
    {
        currencyManager.SetMultiplier(levels - currentLevel);

        if (!isResuming)
        {
            GetNewLevelInfo();
        }
        else
        {
            isResuming = false;
        }
        var newLevelManager = ResourceLoader.Load(path) as PackedScene;
        currentLevelMngr = newLevelManager.Instance() as LevelManager;
        AddChild(currentLevelMngr);
        currentLevelMngr.myGameManager = this;
        if(myGameGen.bonusGenerated && currentLevel == 8)
        {
            currentLevelMngr.SetBonusPosition(myGameGen.bonusSlot);
            currentLevelMngr.gotABonus = true;
        }
        currentLevelMngr.SetActivesPositions(currentLevelInfo, badOnes[levels - currentLevel]);
        currentLevel--;
    }

    public void CheckHexSelected(bool win, String nodeName, bool bonus)
    {
        UpdateSaveData(nodeName);
        
        if (win)
        {
            EmitSignal(nameof(RoundWined));
            if(bonus)
            {
                gotBonus = true;
                GD.Print("El nodo " + nodeName + " tenía Bonus!");
            }
            if (currentLevel <= 0)
            {
                EndGame(true);
            }
            else
            {
                CheckToFinishGame();
                CreateTimer(.4f, "CreateCurrentLevel");
            }
        }
        else
        {
            EndGame(false);
        }
    }
    void CheckToFinishGame()
    {
        if (currentLevel <= 8)
        {
            EmitSignal(nameof(CanCollect));
            EmitSignal(nameof(StartTimer), timerInLevel);
        }
    }
    private void InstantiateBonus()
    {
        EmitSignal(nameof(StartBonus));
        CreateBonusLevel(Constants.PATH_BONUS);
    }

    public void StartGame() //La llama UIManager, señal RestartGame
    {
        if (!isResuming)
        {
            bet_description = "P";
        }
        isPlaying = true;
        EmitSignal(nameof(GameStarted));
        if (currentLevelMngr != null)
        {
            currentLevelMngr.QueueFree();
        }
        if (!isResuming)
        {
            currentLevel = 10;
            CreateLevel(Constants.PATH_LEVEL_MANAGER);
        }
    }
    void CreateTimer(float secs, string method)
    {
        var timer = new Timer();
        timer.WaitTime = secs;
        timer.OneShot = true;
        AddChild(timer);
        timer.Start();
        timer.Connect("timeout", this, method);
    }

    void CreateCurrentLevel() //Se llama dentro de la función CheckHexsSelected, la recibe CreateTimer
    {
        CreateLevel(Constants.PATH_LEVEL_MANAGER);
    }
    void GameHaveBetNow(bool haveBet) //La llama CurrencyManager, señal GameHaveBet
    {
        if (!isPlaying)
        {
            EmitSignal(nameof(GameReady), haveBet);
        }
    }
    void GetNewLevelInfo()
    {
        myGameGen.SetBadOnes(badOnes[levels - currentLevel]);
        currentLevelInfo = myGameGen.GenerateLevelInfo(currentLevel);
        bet_description += myGameGen.levelDescription;
        UpdateSaveData(null);
    }
    void MoneyCollected() //La llama la señal Collect, del UIManager
    {
        if (isPlaying)
        {
            EndGame(true);
        }
    }

    public void EndGame(bool win)
    {
        if(gotBonus)
        {
            EmitSignal(nameof(StartBonus));
        }
        else
        {
            gotBonus = false;
            myGameGen.ResetBonus(); // Game Generator debería saber cuándo tiene que resetear su bonus, (Game Over)
            if (!win)
            {
                // ESTO SE HACE DESPUES DE JUGAR EL BONUS!
                GD.Print("Perdí, generando " + (currentLevel) + " niveles faltantes");
                FillBetDescription(currentLevel);
            }
            isPlaying = false;
            EmitSignal(nameof(GameOver), win);
            bet_description = "";
            currentLevelMngr.ExitAnimation();

            if (UseDB)
            {
                OMenuClient.Structs.SaveData saveData = new OMenuClient.Structs.SaveData(isPlaying, currencyManager.credit, currencyManager.currentBet, DateTime.Now, bet_description);
                oMenu.UpdateSaveData(saveData, null, null);
            }
        }

        
    }


    void ResumeCrashedGame()
    {
        isResuming = true;
        currentLevel = myRecover.GetLevelReached();

        myGameGen.bonusGenerated = myRecover.GetPossibleBonus();
        if(myGameGen.bonusGenerated)
        {
            myGameGen.bonusSlot = myRecover.bonuSlot;
            gotBonus = myRecover.GetIfBonusGained();
            myGameGen.bonusAssigned = true;
        }

        if(currentLevel > 0)
        {
            CheckToFinishGame();
            currencyManager.SetMultiplier((levels - currentLevel) - 1);
            currentLevelInfo = myRecover.GetLastLevelInfo(currentLevel);
            EmitSignal(nameof(RoundWined));
            StartGame();
            CreateCurrentLevel();
        }
        else if(gotBonus)
        {
            GD.Print("Resuming Bonus");
            StartGame();
            EmitSignal(nameof(StartBonus));
            gotBonus = false;
        }
        
    }
}
