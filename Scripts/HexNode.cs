using Godot;
using System;

public class HexNode : Node
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
    public bool goodOne = true;
    public bool asigned;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }

    void _on_Button_button_down()
    {
        if(goodOne)
        {
            GD.Print("Soy de los buenos :D");
        }
        else
            GD.Print("Soy de los malos :c");

    }
}
