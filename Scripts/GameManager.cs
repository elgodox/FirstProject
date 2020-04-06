using Godot;
using System;
using System.Collections;
using System.Collections.Generic;

public class GameManager : Godot.Control
{
	[Signal] public delegate void GameOver(bool win);
	[Signal] public delegate void GameStarted();
	[Signal] public delegate void RoundWined();
	[Signal] public delegate void SetCurrencyManager(double credit,double minBet, double maxBet);
	[Signal] public delegate void GameReady(bool ready);

	Dictionary<string, double> myCurrencyValues = new Dictionary<string, double>();

	private PackedScene hexManagerScn;
	public HexManager currentHexMngr;
	int currentLevel;
	string bet_description;
	string nodePressed;
	[Export] int levels;
	//[Export] int badOnes;
	[Export] int[] badOnes = new int [10];
	bool isPlaying = false;

	DateTime dateTime;
	OMenuCommunication oMenu = new OMenuCommunication();
	GameGenerator myGameGen = new GameGenerator();
	CurrencyManager currencyManager;
	int[] currentLevelInfo;

	public override void _Ready()
	{
		currencyManager = GetNode(Constants.currency_Manager_path) as CurrencyManager;
        if(oMenu.Start())
		{
			EmitSignal(nameof(SetCurrencyManager),oMenu.GetMoney(),oMenu.MinBet(),oMenu.MaxBet());
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
		// Random randomNumber = new Random();
		// int randomLevel = (randomNumber.Next() % 4);
		// randomLevel = Mathf.Clamp(randomLevel, 1 , 4);
		// GD.Print(randomLevel);

		// if(currentLevel >= 1)
		// {
		// 	return "res://Prefabs/Levels/" + currentLevel + "/HexManager" + currentLevel + "_" + randomLevel + ".tscn";
		// }
		// else
		return "res://Prefabs/HexManager.tscn";
		
	}

	public void CheckHexSelected(bool win, String nodeName)
	{
		nodePressed=nodeName;
		if(win)
		{
			EmitSignal(nameof(RoundWined));
			UpdateSaveData();
			
			if(currentLevel <= 0)
			{
				EmitSignal(nameof(GameOver), true);
				isPlaying = false;
				UpdateSaveData();
			}
			else
			{
				CreateTimer(.4f, "CreateHexWithCurrentLevel");
			}
		}
		else
		{
			EmitSignal(nameof(GameOver), false);
			isPlaying = false;
		}
	}
	public void StartGame() //La llama UIManager, señal RestartGame
	{
		isPlaying = true;
		EmitSignal(nameof(GameStarted));

		if(currentHexMngr != null)
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

	void UpdateSaveData()
	{
		bet_description+=myGameGen.levelDescription;
		bet_description+="|";
		bet_description+=nodePressed;
		bet_description+=";";
		oMenu.UpdateSaveData(isPlaying, currencyManager.credit, currencyManager.currentBet, dateTime, bet_description);
	}

	void CreateHexWithCurrentLevel() //Se llama dentro de la función CheckHexsSelected, la recibe CreateTimer
	{
		CreateHex(SceneGenerator(currentLevel));
	}
	void GameHaveBetNow(bool haveBet) //La llama CurrencyManager, señal GameHaveBet
	{
		if(!isPlaying)
		{
			EmitSignal(nameof(GameReady), true);
		}
	}

	void GetNewLevelInfo()
	{
		myGameGen.SetBadOnes(badOnes[levels - currentLevel]);
		currentLevelInfo = myGameGen.GenerateLevelInfo(currentLevel);
	}
}
