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
	[Signal] public delegate void SetCurrencyManager(double credit,double minBet, double maxBet);
	[Signal] public delegate void GameReady(bool ready);

	Dictionary<string, double> myCurrencyValues = new Dictionary<string, double>();

	private PackedScene hexManagerScn;
	public HexManager currentHexMngr;
	int currentLevel;
	[Export] int levels;
	//[Export] int badOnes;
	[Export] int[] badOnes = new int [10];
	bool isPlaying = false;

	OMenuCommunication oMenu = new OMenuCommunication();
	GameGenerator myGameGen = new GameGenerator();

	int[] currentLevelInfo;

	public override void _Ready()
	{
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

	public void CheckHexSelected(bool win)
	{
		if(win)
		{
			EmitSignal(nameof(RoundWined));
			
			if(currentLevel <= 0)
			{
				EmitSignal(nameof(GameOver), true);
				isPlaying = false;
			}
			else
			{	
				if(currentLevel <= 8) 
				{ 
					EmitSignal(nameof(CanCollect)); 
				}

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

	void UpdateSaveData(double moneyAmount, double betAmount, DateTime DateTime)
	{
		oMenu.UpdateSaveData(isPlaying, moneyAmount, betAmount, DateTime, currentLevel.ToString());
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

	void UpdateCurrency(string nameOfCurrency, double currencyValue)
	{
		
	}

	void GetNewLevelInfo()
	{
		myGameGen.SetBadOnes(badOnes[levels - currentLevel]);
		currentLevelInfo = myGameGen.GenerateLevelInfo(currentLevel);
	}

	void MoneyCollected()
	{
		if(isPlaying)
		{
			EmitSignal(nameof(GameOver), false);
			currentHexMngr.DestroyHexManager();
			isPlaying = false;
		}
		//oMenu.
	}
}
