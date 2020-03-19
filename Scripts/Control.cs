using Godot;
using System;

public class Control : Godot.Control
{

    private PackedScene hexManagerScn;
    [Export] int level;
    public override void _Ready()
    {
       GD.Print(GetChildCount());
    }

    public override void _Process(float delta)
    {
        
        
        
        if(GetChildCount()==2)
        {
            GD.Print(GetChild(0).Name);
            CreateHex(SceneGenerator());
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

    String SceneGenerator()
    {
        if(level==10)
        {
            return "res://Prefabs/HexManager10.tscn";
        }
        if(level==9)
        {
            return "res://Prefabs/HexManager9.tscn";
        }
        if(level==8)
        {
            return "res://Prefabs/HexManager8.tscn";
        }
        if(level==7)
        {
            return "res://Prefabs/HexManager7.tscn";
        }
        if(level==6)
        {
            return "res://Prefabs/HexManager6.tscn";
        }
        if(level==5)
        {
            return "res://Prefabs/HexManager5.tscn";
        }
        if(level==4)
        {
            return "res://Prefabs/HexManager4.tscn";
        }
        if(level==3)
        {
            return "res://Prefabs/HexManager3.tscn";
        }
        if(level==2)
        {
            return "res://Prefabs/HexManager2.tscn";
        }
        else
        return "res://Prefabs/HexManager.tscn";

    }


}
