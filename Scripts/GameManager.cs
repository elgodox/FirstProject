using Godot;
using System;
public class GameManager : Godot.Control
{

    private PackedScene hexManagerScn;
    private PackedScene GameOverWin;
    private PackedScene GameOverLose;
    [Export] int level;
    public override void _Ready()
    {
        
    }

    public override void _Process(float delta)
    {
        if(GetChildCount()==2)
        {
            CreateHex(SceneGenerator(level));
            level--;
        }
        
    }

    void CreateHex(String path)
    {
        hexManagerScn = ResourceLoader.Load(path) as PackedScene;
        HexManager hexMng;
        hexMng = hexManagerScn.Instance() as HexManager;
        AddChild(hexMng);
        hexMng.myGameManager = this;
    }

    PackedScene PrepareScene(PackedScene p, string path)
    {
        p = ResourceLoader.Load(path) as PackedScene;
        return p;
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

    public void GameOver(bool win)
    {
        if(win)
        {
            GD.Print("Game Over: Ganaste!");
            //GameOverWin = ResourceLoader.Load(Constants.ui_GmOvr_win_path) as PackedScene;
            //GameOverWin.Instance();
        }
        else
        {
            GD.Print("Game Over: Perdiste!");
            GameOverLose = ResourceLoader.Load(Constants.ui_GmOvr_lose_path) as PackedScene;
            Control c = GameOverLose.Instance() as Control;
            AddChild(c);
        }
    }

}
