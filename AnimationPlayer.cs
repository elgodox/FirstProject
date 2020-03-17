using Godot;
using System;

public class AnimationPlayer : Godot.AnimationPlayer
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
    }

    public void _on_TextureButton_button_down()
    {
        CurrentAnimation = "Pressed";
    }

    public void _on_TextureButton_button_up()
    {
        CurrentAnimation = "NotPressed";
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
