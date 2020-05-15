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
    [Signal] public delegate void NodePicked();
    [Signal] public delegate void LevelCreated();

    public LevelManager currentLevelMngr;
    public int currentLevel;
    string _betDescription;
    [Export] bool _useDb = false;
    bool _gotBonus = false;
    [Export] int _levels, _timerInLevel;
    [Export] int[] _badOnes = new int[10];
    bool _isPlaying = false;
    GameRecover _myRecover;
    OMenuCommunication _oMenu = new OMenuCommunication();
    GameGenerator _myGameGen = new GameGenerator();
    CurrencyManager _currencyManager;
    Panner _panner;
    int[] _currentLevelInfo;
    bool _isResuming;

    PackedScene levelManagerPrefab;
    PackedScene bonusManagerPrefab;
    
    Timer timer;

    public override void _Ready()
    {
        levelManagerPrefab = ResourceLoader.Load(Constants.PATH_LEVEL_MANAGER) as PackedScene;
        bonusManagerPrefab = ResourceLoader.Load(Constants.PATH_BONUS) as PackedScene;
        
        IntiTimer();

        _currencyManager = GetNode("CurrencyManager") as CurrencyManager;
        _panner = GetNode("PannerAnimation") as Panner;

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

    void CreateBonusLevel()
    {
        currentLevelMngr = bonusManagerPrefab.Instance() as LevelManager;
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
        }

        var bonusMngr = currentLevelMngr as BonusManager;
        bonusMngr.SetMultipliersPositions(_currentLevelInfo);
    }

    void CreateLevel()
    {
        CheckToFinishGame();
        _currencyManager.SetLevelMultiplier(_levels - currentLevel);

        if (!_isResuming)
        {
            GetNewLevelInfo();
        }
        
        currentLevelMngr = levelManagerPrefab.Instance() as LevelManager;
        AddChild(currentLevelMngr);
        currentLevelMngr.myGameManager = this;
        if(_myGameGen.bonusGenerated && currentLevel == 8)
        {
            currentLevelMngr.SetBonusPosition(_myGameGen.bonusSlot);
            currentLevelMngr.gotABonus = true;
        }
        currentLevelMngr.SetActivesPositions(_currentLevelInfo, _badOnes[_levels - currentLevel]);
        currentLevel--;
        //New level created
        EmitSignal(nameof(LevelCreated));
    }

    public void CheckHexSelected(bool win, string nodeName, bool bonus)
    {
        //NodePicked
        EmitSignal(nameof(NodePicked));
        UpdateSaveData(nodeName);
        
        if (win)
        {
            EmitSignal(nameof(RoundWined));
            _panner.Panning(currentLevel);
            if(bonus)
            {
                _gotBonus = true;
                EmitSignal(nameof(NodeWithBonus));
                SetTimeOutMethod(1.5f, "CreateLevel");
            }
            else if (currentLevel <= 0)
            {
                EndGame(true);
            }
            else
            {
                
                SetTimeOutMethod(.65f, "CreateLevel");
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
            EmitSignal(nameof(StartTimer), _timerInLevel);
        }
    }
    public void InstantiateBonus() // La llama UI Manager, señal BonusAccepted
    {
        CreateBonusLevel();
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
            currentLevel = 10;
            EmitSignal(nameof(PlayMusicGame));
            CreateLevel();
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

    void GameHaveBetNow(bool haveBet) //La llama CurrencyManager, señal GameHaveBet
    {
        if (!_isPlaying)
        {
            EmitSignal(nameof(GameReady), haveBet);
        }
    }
    void GetNewLevelInfo()
    {
        _myGameGen.SetBadOnes(_badOnes[_levels - currentLevel]);
        _currentLevelInfo = _myGameGen.GenerateLevelInfo(currentLevel);
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
        GD.Print("End Game, el nivel actual es " + currentLevel + " y el bool de win es " + win);
        currentLevelMngr?.ExitAnimation();
        _currencyManager.UpdateWinnedCurrency();
        EmitSignal(nameof(LevelsOver), win, _gotBonus);
        if (!win)
        {
            _currencyManager.ResetCurrencyToCollect();
        }
        if (currentLevel > 0)
        {
            if (win)
            {
                //GD.Print("Gané pero el nivel actual" + _currentLevel +" es mayor a 0, por lo que o se me acabó el tiempo, o finalicé sin apretar ningún nodo.");
                _betDescription += "-1;";
            }
            //GD.Print("Finalicé antes de llegar al final, generando " + (_currentLevel) + " niveles faltantes");
            FillBetDescription(currentLevel);
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
        GameHaveBetNow(false);
        EmitSignal(nameof(BonusOver));
    }
    
    void GameCompletelyOver()
    {
        GD.Print("gameCompletely Over");
        _myGameGen.ResetBonus();
        _gotBonus = false;
        _isPlaying = false;
        _betDescription = "";
        if (_useDb)
        {
            OMenuClient.Structs.SaveData saveData = new OMenuClient.Structs.SaveData(_isPlaying, _currencyManager.credit, _currencyManager.currentBet, DateTime.Now, _betDescription);
            _oMenu.UpdateSaveData(saveData, null, null);
        }
        EmitSignal(nameof(GameOver));
    }

    void ResumeCrashedGame()
    {
        _isResuming = true;
        currentLevel = _myRecover.GetLevelReached();

        _myGameGen.bonusGenerated = _myRecover.GetPossibleBonus();
        
        if(_myGameGen.bonusGenerated)
        {
            _myGameGen.bonusSlot = _myRecover.bonuSlot;
            _gotBonus = _myRecover.GetIfBonusGained();
            _myGameGen.bonusAssigned = true;
        }

        StartGame();
        
        if(currentLevel > 0)
        {
            CheckToFinishGame();
            _currencyManager.SetLevelMultiplier((_levels - currentLevel) - 1);
            _currentLevelInfo = _myRecover.GetLastLevelInfo(currentLevel);
            EmitSignal(nameof(RoundWined));
            _panner.Panning(currentLevel);
            CreateLevel();
            EmitSignal(nameof(PlayMusicGame));
            if (_gotBonus)
            {
                EmitSignal(nameof(NodeWithBonus));
            }
        }
        else if(_gotBonus)
        {
            GD.Print("Resuming Bonus");
            _currencyManager.SetLevelMultiplier((_levels - _myRecover.GetLastLevelWinned()));
            _currencyManager.AddBetToCurrency();
            EndGame(_myRecover.CheckIfWin());
        }

        _isResuming = false;
    }
}
