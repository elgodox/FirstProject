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

    Dictionary<string, double> myCurrencyValues = new Dictionary<string, double>();

    private PackedScene hexManagerScn;
    public HexManager currentHexMngr;
    int currentLevel;
    string bet_description = "P";
    string nodePressed;
    [Export] int levels;
    [Export] int[] badOnes = new int[10];
    bool isPlaying = false;
    GameRecover myRecover;
    DateTime dateTime;
    OMenuCommunication oMenu = new OMenuCommunication();
    GameGenerator myGameGen = new GameGenerator();
    CurrencyManager currencyManager;
    int[] currentLevelInfo;
    Timer timerAutoEnd = new Timer();

    public override void _Ready()
    {

        currencyManager = GetNode(Constants.currency_Manager_path) as CurrencyManager;
        if (oMenu.Start())
        {
            if (oMenu.IsPlaying())
            {
                CheckBetDescription();

            }
            EmitSignal(nameof(SetCurrencyManager), oMenu.GetMoney(), oMenu.MinBet(), oMenu.MaxBet());
        }
    
        AddChild(timerAutoEnd);
        timerAutoEnd.Connect("timeout", this, "MoneyCollected");

    }

    void CheckBetDescription()
    {
        string betToCheck = oMenu.GetBetDescription();
        if (betToCheck != null)
        {
            if (betToCheck.Contains("P"))
            {
                bet_description = betToCheck;
                myRecover = new GameRecover(bet_description);
                myRecover.FillDictionarys(levels);
            }
        }

    }

    public override void _Process(float delta)
    {

    }

    void CreateHex(String path)
    {
        GetNewLevelInfo();
        hexManagerScn = ResourceLoader.Load(path) as PackedScene;
        currentHexMngr = hexManagerScn.Instance() as HexManager;
        AddChild(currentHexMngr);
        currentHexMngr.myGameManager = this;
        currentHexMngr.SetActivesPositions(currentLevelInfo, badOnes[currentLevel - 1]);
        currentLevel--;
    }

    String SceneGenerator(int currentLevel)
    {
        return "res://Prefabs/HexManager.tscn";
    }

    public void CheckHexSelected(bool win, String nodeName)
    {
        nodePressed = nodeName;
        UpdateSaveData(nodePressed);
        if (win)
        {
            EmitSignal(nameof(RoundWined));
            if (currentLevel <= 0)
            {
                EmitSignal(nameof(GameOver), true);
                EndGame();
            }
            else
            {
                if (currentLevel <= 8)
                {
                    EmitSignal(nameof(CanCollect));
                    timerAutoEnd.WaitTime = 5f;
                    timerAutoEnd.OneShot = true;
                    timerAutoEnd.Start();
                }

                CreateTimer(.4f, "CreateHexWithCurrentLevel");
            }
        }
        else
        {
            EmitSignal(nameof(GameOver), false);
            EndGame();
        }
    }
    public void StartGame() //La llama UIManager, señal RestartGame
    {
        isPlaying = true;
        EmitSignal(nameof(GameStarted));

        if (currentHexMngr != null)
        {
            currentHexMngr.QueueFree();
        }
        CreateHex(SceneGenerator(currentLevel = 10));
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

    void UpdateSaveData(String nodePressed)
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
        oMenu.UpdateSaveData(isPlaying, currencyManager.credit, currencyManager.currentBet, dateTime, bet_description);
    }

    void CreateHexWithCurrentLevel() //Se llama dentro de la función CheckHexsSelected, la recibe CreateTimer
    {
        CreateHex(SceneGenerator(currentLevel));

    }
    void GameHaveBetNow(bool haveBet) //La llama CurrencyManager, señal GameHaveBet
    {
        if (!isPlaying)
        {
            EmitSignal(nameof(GameReady), true);
        }
    }

    void GetNewLevelInfo()
    {
        myGameGen.SetBadOnes(badOnes[levels - currentLevel]);
        currentLevelInfo = myGameGen.GenerateLevelInfo(currentLevel);
        bet_description += myGameGen.levelDescription;
        UpdateSaveData(null);
    }

    void MoneyCollected()
    {
        if (isPlaying)
        {
            EmitSignal(nameof(GameOver), true);
            currentHexMngr.DestroyHexManager();
            EndGame();
        }
        //oMenu.
    }
    void EndGame()
    {
        GD.Print("EndGame");
        isPlaying = false;
        bet_description = "";
        oMenu.UpdateSaveData(isPlaying, currencyManager.credit, currencyManager.currentBet, dateTime, bet_description);
    }


}
