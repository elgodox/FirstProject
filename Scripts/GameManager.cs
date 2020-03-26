using Godot;
using System;
using System.Collections;

public class GameManager : Godot.Control
{
    
    private PackedScene hexManagerScn;
    private PackedScene gameOverWin;
    private PackedScene gameOverLose;
    HexManager currentHexMngr;
    Control gameOverMessage;
    [Export] int level;
    public override void _Ready()
    {
        CreateHex(SceneGenerator(level));
    }

    public override void _Process(float delta)
    {
        
    }

    void CreateHex(String path)
    {
        hexManagerScn = ResourceLoader.Load(path) as PackedScene;
        currentHexMngr = hexManagerScn.Instance() as HexManager;
        currentHexMngr.badOnes = 0;
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
            if(level < 2)
            {
                SetGameOverMessage(gameOverWin, Constants.ui_GmOvr_win_path);
            }
            else
            {
                CreateTimer(.5f, "CreateHexWithCurrentLevel");
            }
        }
        else
        {
            SetGameOverMessage(gameOverLose, Constants.ui_GmOvr_lose_path);
        }
    }

    void SetGameOverMessage(PackedScene scene, string pathScene)
    {
        scene = ResourceLoader.Load(pathScene) as PackedScene;
        gameOverMessage = scene.Instance() as Control;
        AddChild(gameOverMessage);
    }
    public void RestartGame()
	{
        if(level > 1)
        {
            currentHexMngr.QueueFree();
        }
        gameOverMessage.QueueFree();
		CreateHex(SceneGenerator(level = 10));
        GD.Print("Juego Reiniciado");
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
    public IEnumerator TestCoroutine()
    {
        GD.Print("asd");
        
        var t = new Timer();
        GD.Print("Comenzando cuenta regresiva: " + t + "Segundos");
        t.WaitTime = 1;
        t.OneShot = true;
        this.AddChild(t);
        t.Start();
        yield return (t, "timeout");
        t.QueueFree();
    }
    //Habia un bug que podías seguir apretando HexNodes mientras se estaba hacien do la animacion de cambio de nivel
    //Hice el canvas de la pantalla de Win, que se escale a la pantalla
    //Hice que el botón de Play Reinicie el juego, todavia tengo que desactivarlo
    //Arranqué a hacer un timer y viendo corrutinas
}
