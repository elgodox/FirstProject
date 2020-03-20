using Godot;
using System;

public class Control : Godot.Control
{

    private PackedScene hexManagerScn;
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
        HexManager e;
        e = hexManagerScn.Instance() as HexManager;
        AddChild(e);
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


}
