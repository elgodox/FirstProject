using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using OMenuClient.Structs;

public class GameManager : Godot.Control
{
    [Signal]
    public delegate void CanCollect();

    [Signal]
    public delegate void GameOver();

    [Signal]
    public delegate void LevelsOver(bool win, bool bonus);

    [Signal]
    public delegate void GameStarted();

    [Signal]
    public delegate void PlayMusicGame();

    [Signal]
    public delegate void RoundWined();

    [Signal]
    public delegate void SetCurrencyManager(double credit, double minBet, double maxBet);

    [Signal]
    public delegate void GameReady(bool ready);

    [Signal]
    public delegate void StartTimer(float secs);

    [Signal]
    public delegate void BonusStarted();

    [Signal]
    public delegate void NodeWithBonus();

    [Signal]
    public delegate void BonusOver();

    [Signal]
    public delegate void NodePicked();

    [Signal]
    public delegate void DemoModeStarted();

    [Signal]
    public delegate void LevelCreated();

    [Signal]
    public delegate void RemoveBonusMessage();

    [Signal]
    public delegate void StopGameInmediate();

    public LevelManager currentLevelMngr;
    public int currentLevel;
    string _betDescription;
    [Export] bool _useDb = false;
    [Export] float timeToSetDemo = 2;
    [Export] float timeToDemoActions = 3;
    [Export] int _levels, _timeInLevel;
    [Export] int[] _badOnes = new int[10];
    int[] _currentLevelInfo;
    bool _isPlaying, _gotBonus, _isResuming, _isDemo, _wasUsingDb;
    GameRecover _myRecover;
    OMenuCommunication _oMenu = new OMenuCommunication();
    GameGenerator _myGameGen = new GameGenerator();
    CurrencyManager _currencyManager;
    Panner _panner;
    Control _barrier;

    PackedScene levelManagerPrefab;
    BonusManager bonusManager;

    Timer _timerToCreateLevels, _timerToDemoMode, _actionsTimer;

    public override void _Ready()
    {
        InitChilds();

        if (_useDb)
        {
            _oMenu.Start();
            _currencyManager.currentBet = _oMenu.GetCurrentBet();
            EmitSignal(nameof(SetCurrencyManager), _oMenu.GetMoney(), _oMenu.MinBet(), _oMenu.MaxBet());
            CheckBetDescription(GetBetDescription());
            //GD.Print("omenu credit: " + _oMenu.GetMoney());
        }
        else
        {
            EmitSignal(nameof(SetCurrencyManager), 1000, 5, 25);
        }

        if (!_isPlaying)
        {
            _currencyManager.BetUp();
            _timerToDemoMode.Start();
        }
    }

    void InitChilds()
    {
        levelManagerPrefab = ResourceLoader.Load(Constants.PATH_LEVEL_MANAGER) as PackedScene;
        var bonusScene = ResourceLoader.Load(Constants.PATH_BONUS) as PackedScene;

        bonusManager = bonusScene.Instance() as BonusManager;
        GetNode("UI_Template").AddChildBelowNode(GetNode("UI_Template/AnimationPlayer"), bonusManager);
        bonusManager.myGameManager = this;
        _barrier = GetNode("Barrier") as Control;
        _currencyManager = GetNode("CurrencyManager") as CurrencyManager;
        _panner = GetNode("PannerAnimation") as Panner;
        InitTimers();
    }

    void InitTimers()
    {
        _timerToCreateLevels = new Timer();
        _timerToDemoMode = new Timer();
        _actionsTimer = new Timer();

        AddChild(_timerToCreateLevels);
        AddChild(_timerToDemoMode);
        AddChild(_actionsTimer);
        _actionsTimer.Name = nameof(_actionsTimer);
        _timerToDemoMode.Name = nameof(_timerToDemoMode);
        _timerToCreateLevels.Name = nameof(_timerToCreateLevels);

        _timerToCreateLevels.OneShot = true;
        _timerToDemoMode.OneShot = true;
        _actionsTimer.OneShot = true;

        _timerToDemoMode.WaitTime = timeToSetDemo;
        _actionsTimer.WaitTime = timeToDemoActions;
        _timerToDemoMode.Connect("timeout", this, nameof(SetDemoMode));
    }

    #region BBDD Methods

    private string GetBetDescription()
    {
        return _oMenu.GetBetDescription();
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
                ResumeCrashedGame();
            }
            else
            {
                UpdateSaveDataLocal();
            }
        }
    }

    void UpdateSaveDataLocal()
    {
        double money = _oMenu.GetMoney();
        double bet = _oMenu.GetCurrentBet();
        double betToMoney = bet + money;
        OMenuClient.Structs.SaveData newSaveData =
            new OMenuClient.Structs.SaveData(false, betToMoney, 0, DateTime.Now, "");
        _oMenu.UpdateSaveData(newSaveData);
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

            OMenuClient.Structs.SaveData saveData = new OMenuClient.Structs.SaveData(_isPlaying,
                _currencyManager.credit, _currencyManager.currentBet, DateTime.Now, _betDescription);
            _oMenu.UpdateSaveData(saveData);
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
    }

    #endregion

    void CreateBonusLevel()
    {
        currentLevelMngr = bonusManager;

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

        bonusManager.SetMultipliersPositions(_currentLevelInfo);
        bonusManager.PlayInitAnimation();
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

        GetNode("UI_Template").AddChildBelowNode(GetNode("UI_Template/AnimationPlayer"), currentLevelMngr);

        currentLevelMngr.myGameManager = this;
        if (_myGameGen.bonusGenerated && currentLevel == 8)
        {
            currentLevelMngr.SetBonusPosition(_myGameGen.bonusSlot);
            currentLevelMngr.gotABonus = true;
        }

        currentLevelMngr.SetActivesPositions(_currentLevelInfo, _badOnes[_levels - currentLevel]);
        currentLevel--;
        //New level created
        EmitSignal(nameof(LevelCreated));

        if (_isDemo)
        {
            _actionsTimer.WaitTime = timeToDemoActions;
            if (_actionsTimer.IsConnected("timeout", this, nameof(StartGame)))
            {
                _actionsTimer.Disconnect("timeout", this, nameof(StartGame));
            }

            _actionsTimer.Connect("timeout", currentLevelMngr, nameof(currentLevelMngr.ChooseRandomNode));
            _actionsTimer.Start();
        }
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
            if (bonus)
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
            EmitSignal(nameof(StartTimer), _timeInLevel);
        }
    }

    public void InstantiateBonus() // La llama UI Manager, señal BonusAccepted
    {
        CreateBonusLevel();
        EmitSignal(nameof(BonusStarted));
        if (_isDemo)
        {
            _actionsTimer.Disconnect("timeout", this, nameof(InstantiateBonus));
            _actionsTimer.Connect("timeout", currentLevelMngr, nameof(currentLevelMngr.ChooseRandomNode));
            _actionsTimer.Start();
        }
    }

    public void CashOut()
    {
        if (_useDb)
        {
            _oMenu.CashOut(_oMenu.GetMoney());
            OMenuClient.Structs.SaveData saveData =
                new OMenuClient.Structs.SaveData(_isPlaying, 0, 0, DateTime.Now, "");
            _oMenu.UpdateSaveData(saveData);
            EmitSignal(nameof(SetCurrencyManager), _oMenu.GetMoney(), _oMenu.MinBet(), _oMenu.MaxBet());
            _currencyManager.CheckBet();
        }
        else
        {
            EmitSignal(nameof(SetCurrencyManager), 1000, 5, 25);
        }
    }

    public void BackToMenu()
    {
        if (_useDb)
        {
            _oMenu.GoToMenu();
        }
    }

    public void StartGame() //La llama UIManager, señal RestartGame
    {
        _timerToDemoMode.Stop();
        _isPlaying = true;
        EmitSignal(nameof(GameStarted));
        // if (currentLevelMngr != null)
        // {
        //     currentLevelMngr.QueueFree();
        // }
        if (!_isResuming)
        {
            _currencyManager.ConfirmBet();
            _betDescription = "P";
            currentLevel = 10;
            EmitSignal(nameof(PlayMusicGame));
            CreateLevel();
        }
    }

    void SetTimeOutMethod(float secs, string method)
    {
        _timerToCreateLevels.Stop();
        _timerToCreateLevels.WaitTime = secs;
        if (!_timerToCreateLevels.IsConnected("timeout", this, method))
        {
            _timerToCreateLevels.Connect("timeout", this, method);
        }

        _timerToCreateLevels.Start();
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
        if (_useDb)
            UpdateSaveData(null);
    }

    void GetNewBonusInfo()
    {
        //GD.Print("Generando Nueva info de bonus");
        _currentLevelInfo = _myGameGen.GenerateBonusInfo();
        _betDescription += _myGameGen.GetBonusDescription(_currentLevelInfo);
        if (_useDb)
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
        //GD.Print("End Game, el nivel actual es " + currentLevel + " y el bool de win es " + win);
        currentLevelMngr?.ExitAnimation();

        if (_isDemo)
        {
            _actionsTimer.Stop();
            _actionsTimer.WaitTime = 5;

            if (_actionsTimer.IsConnected("timeout", currentLevelMngr, nameof(currentLevelMngr.ChooseRandomNode)))
            {
                _actionsTimer.Disconnect("timeout", currentLevelMngr, nameof(currentLevelMngr.ChooseRandomNode));
            }

            if (_gotBonus)
            {
                _actionsTimer.Connect("timeout", this, nameof(InstantiateBonus));
            }
            else
            {
                _actionsTimer.Connect("timeout", this, nameof(StartGame));
            }

            _actionsTimer.Start();
        }

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
            GD.Print("EndGame currencymanager, credit: " + _currencyManager.credit);
            OMenuClient.Structs.SaveData saveData = new OMenuClient.Structs.SaveData(_isPlaying,
                _currencyManager.credit, _currencyManager.currentBet, DateTime.Now, _betDescription);
            _oMenu.UpdateSaveData(saveData);
        }

        if (!_gotBonus)
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
        if (_isDemo)
        {
            _actionsTimer.Disconnect("timeout", currentLevelMngr, nameof(currentLevelMngr.ChooseRandomNode));
            _actionsTimer.Connect("timeout", this, nameof(ClearBonusFinished));
            _actionsTimer.WaitTime = 8;
            _actionsTimer.Start();
        }
    }

    void ClearBonusFinished()
    {
        EmitSignal(nameof(RemoveBonusMessage));
        if (_isDemo)
        {
            _actionsTimer.Disconnect("timeout", this, nameof(ClearBonusFinished));
            _actionsTimer.Connect("timeout", this, nameof(StartGame));
            _actionsTimer.Start();
        }
    }

    void GameCompletelyOver()
    {
        _isPlaying = false;
        EmitSignal(nameof(GameOver));
        _myGameGen.ResetBonus();
        _gotBonus = false;
        _betDescription = "";
        if (_useDb)
        {
            OMenuClient.Structs.SaveData saveData = new OMenuClient.Structs.SaveData(_isPlaying,
                _currencyManager.credit, 0, DateTime.Now, _betDescription);
            _oMenu.UpdateSaveData(saveData);
        }

        if (!_isDemo)
            _timerToDemoMode.Start();
    }

    void SetDemoMode()
    {
        _barrier.Show();
        EmitSignal(nameof(RemoveBonusMessage));
        if (_useDb)
        {
            _wasUsingDb = true;
            _useDb = false;
        }

        _isDemo = true;
        EmitSignal(nameof(SetCurrencyManager), 1000, 5, 25);
        _timerToDemoMode.Stop();
        GD.Print("Setting Demo Mode");
        EmitSignal(nameof(DemoModeStarted));
        _actionsTimer.Connect("timeout", this, nameof(StartGame));
        _actionsTimer.WaitTime = 4;
        _actionsTimer.Start();
    }

    void ExitDemoMode() // La llama UI, señal DemoModeFinished
    {
        GD.Print("DemoMode Stoped");
        _isDemo = false;

        _actionsTimer.Stop();
        StopGame();

        DisconnectAllSignalsFromActionsTimer();

        _actionsTimer.OneShot = true;
        _actionsTimer.WaitTime = timeToDemoActions;
        if (_wasUsingDb)
        {
            _wasUsingDb = false;
            _useDb = true;
            EmitSignal(nameof(SetCurrencyManager), _oMenu.GetMoney(), _oMenu.MinBet(), _oMenu.MaxBet());
        }

        _barrier.Hide();
    }

    void DisconnectAllSignalsFromActionsTimer()
    {
        if (_actionsTimer.IsConnected("timeout", this, nameof(StartGame)))
            _actionsTimer.Disconnect("timeout", this, nameof(StartGame));
        if (_actionsTimer.IsConnected("timeout", this, nameof(InstantiateBonus)))
            _actionsTimer.Disconnect("timeout", this, nameof(InstantiateBonus));
        if (_actionsTimer.IsConnected("timeout", this, nameof(ClearBonusFinished)))
            _actionsTimer.Disconnect("timeout", this, nameof(ClearBonusFinished));
        if (currentLevelMngr != null)
        {
            if (_actionsTimer.IsConnected("timeout", currentLevelMngr, nameof(currentLevelMngr.ChooseRandomNode)))
                _actionsTimer.Disconnect("timeout", currentLevelMngr, nameof(currentLevelMngr.ChooseRandomNode));
        }
    }

    void ResumeCrashedGame()
    {
        _isResuming = true;
        currentLevel = _myRecover.GetLevelReached();

        _myGameGen.bonusGenerated = _myRecover.GetPossibleBonus();

        if (_myGameGen.bonusGenerated)
        {
            _myGameGen.bonusSlot = _myRecover.bonuSlot;
            _gotBonus = _myRecover.GetIfBonusGained();
            _myGameGen.bonusAssigned = true;
        }

        StartGame();

        if (currentLevel > 0)
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
        else if (_gotBonus)
        {
            GD.Print("Resuming Bonus");
            _currencyManager.SetLevelMultiplier((_levels - _myRecover.GetLastLevelWinned()));
            _currencyManager.AddBetToCurrency();
            EndGame(_myRecover.CheckIfWin());
        }

        _isResuming = false;
    }

    void StopGame()
    {
        _gotBonus = false;
        EndGame(true);
        _timerToCreateLevels.Stop();
        EmitSignal(nameof(StopGameInmediate));
    }
}