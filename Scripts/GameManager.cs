using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

public class GameManager : Godot.Control
{
    [Signal] public delegate void CanCollect();
    [Signal] public delegate void GameOver();
    [Signal] public delegate void LevelsOver(bool win, bool bonus);
    [Signal] public delegate void GameStarted();
    [Signal] public delegate void PlayMusicGame();
    [Signal] public delegate void RoundWined();
    [Signal] public delegate void SetCurrencyManager(double credit, double minBet, double maxBet);
    [Signal] public delegate void GameReady(bool ready);
    [Signal] public delegate void StartTimer(float secs);
    [Signal] public delegate void BonusStarted();
    [Signal] public delegate void NodeWithBonus();
    [Signal] public delegate void BonusOver();

    public LevelManager currentLevelMngr;
    int _currentLevel;
    string _betDescription;
    [Export] bool _useDb = false;
    bool _gotBonus = false;
    [Export] int _levels, _timerInLevel;
    [Export] int[] _badOnes = new int[10];
    bool _isPlaying = false;
    GameRecover _myRecover;
    OMenuCommunication _oMenu = new OMenuCommunication();
    GameGenerator _myGameGen = new GameGenerator();
    UIButtonsManager _buttonsManager = new UIButtonsManager();
    CurrencyManager _currencyManager;
    int[] _currentLevelInfo;
    bool _isResuming;
    
    
    Timer timer;

    public override void _Ready()
    {
        IntiTimer();

        _currencyManager = GetNode("CurrencyManager") as CurrencyManager;
        _buttonsManager = GetNode("UI_Template") as UIButtonsManager;
        Console.WriteLine(_currencyManager.Name);

        if (_useDb)
        {
            _oMenu.Start(OnStartCompletation, null);
        }
        else
        {
            EmitSignal(nameof(SetCurrencyManager), 1000, 5, 25);
        }
    }

    void IntiTimer()
    {
        timer = new Timer();
        AddChild(timer);
        timer.OneShot = true;
    }

    #region BBDD Methods
    private void OnStartCompletation()
    {
        _oMenu.IsPlaying(delegate { GetBetDescription(delegate (string value) { CheckBetDescription(value); }, null); }, null);
        // Puede llegar a explotar
        _oMenu.GetMoney(delegate (double money) { EmitSignal(nameof(SetCurrencyManager), money, _oMenu.MinBet(), _oMenu.MaxBet()); }, null);
    }
    private void GetBetDescription(Action<string> onSuccess, Action onFail)
    {
        _oMenu.GetBetDescription(onSuccess, onFail);
    }
    void CheckBetDescription(string betToCheck)
    {
        if (betToCheck != null)
        {
            if (betToCheck.Contains("P"))
            {
                _betDescription = betToCheck;
                _myRecover = new GameRecover(_betDescription);
                _myRecover.FillDictionarys(_levels);

                _oMenu.GetMoney(
                    delegate (double money)
                    {
                        _currencyManager.credit = money;

                        _oMenu.GetCurrentBet(delegate (double bet)
                        {
                            _currencyManager.currentBet = bet;
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
        _oMenu.GetSaveData(UpdateSaveDataLocal, null);
    }
    void UpdateSaveDataLocal(OMenuClient.Structs.SaveData saveData)
    {
        double money = saveData.moneyAmount;
        double bet = saveData.betAmount;
        double betToMoney = bet + money;
        OMenuClient.Structs.SaveData newSaveData = new OMenuClient.Structs.SaveData(false, betToMoney, 0, DateTime.Now, "");
        _oMenu.UpdateSaveData(newSaveData, null, null);
    }
    void UpdateSaveData(string nodePressed)
    {
        if (_useDb)
        {
            if (nodePressed != null)
            {
                _betDescription += nodePressed.Replace("Hex", "");
                _betDescription += ";";
            }
            else
            {
                _betDescription += "|";
            }

            OMenuClient.Structs.SaveData saveData = new OMenuClient.Structs.SaveData(_isPlaying, _currencyManager.credit, _currencyManager.currentBet, DateTime.Now, _betDescription);

            _oMenu.UpdateSaveData(saveData, null, null);
        }
    }
    private void FillBetDescription(int restOfLevels)
    {
        if (restOfLevels > 0)
        {
            for (int i = 0; i < restOfLevels; i++)
            {
                var intendedLevel = restOfLevels - i;
                _myGameGen.SetBadOnes(_badOnes[intendedLevel]);
                _currentLevelInfo = _myGameGen.GenerateLevelInfo(intendedLevel);
                string restOfLevelsDescription = _myGameGen.GetLevelDescription(_currentLevelInfo);
                _betDescription += restOfLevelsDescription;
                UpdateSaveData("|-2");
            }
        }
        GD.Print(_betDescription);
    }

    #endregion

    void CreateBonusLevel(string path)
    {
        var newLevelManager = ResourceLoader.Load(path) as PackedScene;
        if (newLevelManager != null) currentLevelMngr = newLevelManager.Instance() as LevelManager;
        AddChild(currentLevelMngr);
        currentLevelMngr.myGameManager = this;
        
        if (!_isResuming)
        {
            GetNewBonusInfo();
        }
        else
        {
            if (_myRecover.GetIfBonusInfoGenerated())
            {
                _currentLevelInfo = _myRecover.GetBonusLevelInfo();
            }
            else
            {
                GetNewBonusInfo();
            }
            _isResuming = false;
        }

        var bonusMngr = currentLevelMngr as BonusManager;
        bonusMngr.SetMultipliersPositions(_currentLevelInfo);
    }

    void CreateLevel(string path)
    {
        _currencyManager.SetMultiplier(_levels - _currentLevel);

        if (!_isResuming)
        {
            GetNewLevelInfo();
        }
        else
        {
            _isResuming = false;
        }
        var newLevelManager = ResourceLoader.Load(path) as PackedScene;
        currentLevelMngr = newLevelManager.Instance() as LevelManager;
        AddChild(currentLevelMngr);
        currentLevelMngr.myGameManager = this;
        if(_myGameGen.bonusGenerated && _currentLevel == 8)
        {
            currentLevelMngr.SetBonusPosition(_myGameGen.bonusSlot);
            currentLevelMngr.gotABonus = true;
        }
        currentLevelMngr.SetActivesPositions(_currentLevelInfo, _badOnes[_levels - _currentLevel]);
        _currentLevel--;
    }

    public void CheckHexSelected(bool win, string nodeName, bool bonus)
    {
        _buttonsManager.StopTimer();
        UpdateSaveData(nodeName);
        
        if (win)
        {
            EmitSignal(nameof(RoundWined));
            if(bonus)
            {
                _gotBonus = true;
                EmitSignal(nameof(NodeWithBonus));
                SetTimeOutMethod(1.5f, "CreateCurrentLevel");
            }
            else if (_currentLevel <= 0)
            {
                EndGame(true);
            }
            else
            {
                CheckToFinishGame();
                SetTimeOutMethod(.65f, "CreateCurrentLevel");
            }
        }
        else
        {
            EndGame(false);
        }
    }
    void CheckToFinishGame()
    {
        if (_currentLevel <= 8)
        {
            EmitSignal(nameof(CanCollect));
            EmitSignal(nameof(StartTimer), _timerInLevel);
        }
    }
    public void InstantiateBonus() // La llama UI Manager, señal BonusAccepted
    {
        CreateBonusLevel(Constants.PATH_BONUS);
        EmitSignal(nameof(BonusStarted));
    }

    public void StartGame() //La llama UIManager, señal RestartGame
    {
        _isPlaying = true;
        EmitSignal(nameof(GameStarted));
        if (currentLevelMngr != null)
        {
            currentLevelMngr.QueueFree();
        }
        if(!_isResuming)
        {
            _betDescription = "P";
            _currentLevel = 10;
            EmitSignal(nameof(PlayMusicGame));
            CreateLevel(Constants.PATH_LEVEL_MANAGER);
        }
    }
    void SetTimeOutMethod(float secs, string method)
    {
        timer.Stop();
        timer.WaitTime = secs;
        if (!timer.IsConnected("timeout", this, method))
        {
            timer.Connect("timeout", this, method);
        }
        timer.Start();
    }

    void CreateCurrentLevel() //Se llama dentro de la función CheckHexsSelected, la recibe CreateTimer
    {
        CreateLevel(Constants.PATH_LEVEL_MANAGER);
    }
    void GameHaveBetNow(bool haveBet) //La llama CurrencyManager, señal GameHaveBet
    {
        if (!_isPlaying)
        {
            EmitSignal(nameof(GameReady), haveBet);
        }
    }
    void GetNewLevelInfo()
    {
        _myGameGen.SetBadOnes(_badOnes[_levels - _currentLevel]);
        _currentLevelInfo = _myGameGen.GenerateLevelInfo(_currentLevel);
        _betDescription += _myGameGen.levelDescription;
        UpdateSaveData(null);
    }

    void GetNewBonusInfo()
    {
        //GD.Print("Generando Nueva info de bonus");
        _currentLevelInfo = _myGameGen.GenerateBonusInfo();
        _betDescription += _myGameGen.GetBonusDescription(_currentLevelInfo);
        UpdateSaveData(null);
    }
    void MoneyCollected() //La llama la señal Collect, del UIManager
    {
        if (_isPlaying)
        {
            EndGame(true);
        }
    }

    public void EndGame(bool win)
    {
        currentLevelMngr?.ExitAnimation();
        _currencyManager.UpdateWinnedCurrency();
        EmitSignal(nameof(LevelsOver), win, _gotBonus);
        if (!win)
        {
            _currencyManager.ResetCurrencyToCollect();
        }
        if (_currentLevel > 0)
        {
            if (win)
            {
                //GD.Print("Gané pero el nivel actual" + _currentLevel +" es mayor a 0, por lo que o se me acabó el tiempo, o finalicé sin apretar ningún nodo.");
                _betDescription += "-1;";
            }
            //GD.Print("Finalicé antes de llegar al final, generando " + (_currentLevel) + " niveles faltantes");
            FillBetDescription(_currentLevel);
        }

        if (_useDb)
        {
            OMenuClient.Structs.SaveData saveData = new OMenuClient.Structs.SaveData(_isPlaying, _currencyManager.credit, _currencyManager.currentBet, DateTime.Now, _betDescription);
            _oMenu.UpdateSaveData(saveData, null, null);
        }
        
        if(!_gotBonus)
        {
            GameCompletelyOver();
        }
    }

    public void BonusFinished(double multiplier)
    {
        _currencyManager.UpdateBonusReward(multiplier);
        GameCompletelyOver();
        EmitSignal(nameof(BonusOver));
    }
    
    void GameCompletelyOver()
    {
        GD.Print("gameCompletely Over");
        EmitSignal(nameof(GameOver));
        _myGameGen.ResetBonus();
        _gotBonus = false;
        _isPlaying = false;
        _betDescription = "";
        if (_useDb)
        {
            OMenuClient.Structs.SaveData saveData = new OMenuClient.Structs.SaveData(_isPlaying, _currencyManager.credit, _currencyManager.currentBet, DateTime.Now, _betDescription);
            _oMenu.UpdateSaveData(saveData, null, null);
        }
    }

    void ResumeCrashedGame()
    {
        _isResuming = true;
        _currentLevel = _myRecover.GetLevelReached();

        _myGameGen.bonusGenerated = _myRecover.GetPossibleBonus();
        
        if(_myGameGen.bonusGenerated)
        {
            _myGameGen.bonusSlot = _myRecover.bonuSlot;
            _gotBonus = _myRecover.GetIfBonusGained();
            _myGameGen.bonusAssigned = true;
        }

        StartGame();
        
        if(_currentLevel > 1)
        {
            CheckToFinishGame();
            _currencyManager.SetMultiplier((_levels - _currentLevel) - 1);
            _currentLevelInfo = _myRecover.GetLastLevelInfo(_currentLevel);
            EmitSignal(nameof(RoundWined));
            CreateCurrentLevel();
            EmitSignal(nameof(PlayMusicGame));
        }
        else if(_gotBonus)
        {
            GD.Print("Resuming Bonus");
            EndGame(_myRecover.CheckIfWin());
        }
        
    }
}
