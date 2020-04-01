using Godot;
using System;
using System.Collections;

public class GameManager : Godot.Control
{
	[Signal] public delegate void GameOver(bool win);
	[Signal] public delegate void GameStarted();
	[Signal] public delegate void RoundWined();

	private PackedScene hexManagerScn;
	public HexManager currentHexMngr;
	[Export] int level;
	public override void _Ready()
	{
        
	}

	public override void _Process(float delta)
	{

	}

	void CreateHex(String path)
	{
		hexManagerScn = ResourceLoader.Load(path) as PackedScene;
		currentHexMngr = hexManagerScn.Instance() as HexManager;
		currentHexMngr.badOnes = 0;
		//AddChildBelowNode(GetNode("Background"), currentHexMngr);
		AddChild(currentHexMngr);
		currentHexMngr.myGameManager = this;
		level--;
	}

	String SceneGenerator(int currentLevel)
	{
		if(currentLevel > 1)
		{
			return "res://Prefabs/HexManager" + currentLevel + ".tscn";
		}
		else
		return "res://Prefabs/HexManager.tscn";
		
	}

	public void CheckHexSelected(bool win)
	{
		if(win)
		{
			EmitSignal(nameof(RoundWined));
			
			if(level < 2)
			{
				EmitSignal(nameof(GameOver), true);
			}
			else
			{
				CreateTimer(.5f, "CreateHexWithCurrentLevel");
			}
		}
		else
		{
			EmitSignal(nameof(GameOver), false);
		}
	}

	public void StartGame()
	{
		EmitSignal(nameof(GameStarted));

		if(currentHexMngr != null)
		{
			currentHexMngr.QueueFree();
		}
		CreateHex(SceneGenerator(level = 10));
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
	void CreateHexWithCurrentLevel()
	{
		CreateHex(SceneGenerator(level));
	}
}
