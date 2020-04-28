using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

public class GameManager : Godot.Control
{
    [Signal] public delegate void CanCollect();
    [Signal] public delegate void GameOver(bool win);
    [Signal] public delegate void LevelsOver(bool win, bool bonus);
    [Signal] public delegate void GameStarted();
    [Signal] public delegate void RoundWined();
    [Signal] public delegate void SetCurrencyManager(double credit, double minBet, double maxBet);
    [Signal] public delegate void GameReady(bool ready);
    [Signal] public delegate void StartTimer(float secs);
    [Signal] public delegate void StartBonus();
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
    CurrencyManager _currencyManager;
    int[] _currentLevelInfo;
    bool _isResuming;

    public override void _Ready()
    {
        _currencyManager = GetNode("CurrencyManager") as CurrencyManager;
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
        GD.Print(_currentLevel);
        UpdateSaveData(nodeName);
        
        if (win)
        {
            EmitSignal(nameof(RoundWined));
            if(bonus)
            {
                _gotBonus = true;
                GD.Print("El nodo " + nodeName + " tenía Bonus!");
            }
            if (_currentLevel <= 0)
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
        if (_currentLevel <= 8)
        {
            EmitSignal(nameof(CanCollect));
            EmitSignal(nameof(StartTimer), _timerInLevel);
        }
    }
    public void InstantiateBonus() // La llama UI Manager, señal BonusAccepted
    {
        GD.Print("Instancia Bonus!");
        CreateBonusLevel(Constants.PATH_BONUS);
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
    void MoneyCollected() //La llama la señal Collect, del UIManager
    {
        if (_isPlaying)
        {
            EndGame(true);
        }
    }

    public void EndGame(bool win)
    {
        currentLevelMngr.ExitAnimation();
        EmitSignal(nameof(LevelsOver), win, _gotBonus);

        if (_currentLevel > 0)
        {
            // ESTO SE HACE DESPUES DE JUGAR EL BONUS!
            GD.Print("Perdí o finalicé antes de llegar al final, generando " + (_currentLevel) + " niveles faltantes");
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
            EmitSignal(nameof(GameOver), win);
        }
    }

    public void BonusFinished()
    {
        EmitSignal(nameof(BonusOver));
        GameCompletelyOver();
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

        if(_currentLevel > 0)
        {
            CheckToFinishGame();
            _currencyManager.SetMultiplier((_levels - _currentLevel) - 1);
            _currentLevelInfo = _myRecover.GetLastLevelInfo(_currentLevel);
            EmitSignal(nameof(RoundWined));
            StartGame();
            CreateCurrentLevel();
        }
        else if(_gotBonus)
        {
            GD.Print("Resuming Bonus");
            StartGame();
            InstantiateBonus();
        }
        
    }
}
