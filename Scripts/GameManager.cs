using Godot;
using System;
using System.Collections;

public class GameManager : Godot.Control
{
	[Signal] public delegate void GameOver(bool win);
	[Signal] public delegate void GameStarted();
	[Signal] public delegate void RoundWined();
	[Signal] public delegate void SetCurrencyManager(double credit,double minBet, double maxBet);
	[Signal] public delegate void GameReady(bool ready);

	private PackedScene hexManagerScn;
	public HexManager currentHexMngr;
	int currentLevel;
	[Export] int Levels;
	bool isPlaying = false;

	OMenuCommunication oMenu = new OMenuCommunication();
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
		hexManagerScn = ResourceLoader.Load(path) as PackedScene;
		currentHexMngr = hexManagerScn.Instance() as HexManager;
		currentHexMngr.badOnes = 0;
		currentHexMngr.activeOnes = currentLevel;
		AddChild(currentHexMngr);
		currentHexMngr.myGameManager = this;
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
			
			if(currentLevel < 1)
			{
				EmitSignal(nameof(GameOver), true);
				isPlaying = false;
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
		CreateHex(SceneGenerator(currentLevel = Levels));
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
}
