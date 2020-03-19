using Godot;
using System;

public class Control : Godot.Control
{

    private PackedScene hexManagerScn;
    public override void _Ready()
    {
       GD.Print(GetChildCount());
    }

    public override void _Process(float delta)
    {
        if(GetChildCount()==2)
        {
            CreateHex();
        }
        
    }

    void CreateHex()
    {
        hexManagerScn = ResourceLoader.Load("res://Prefabs/HexManager.tscn") as PackedScene;
        HexManager e;
        e = hexManagerScn.Instance() as HexManager;
        AddChild(e);
    }


}
