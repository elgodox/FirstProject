using Godot;
using System;

public class HexAnimation : TextureButton
{
	String nameAnimation;
	public override void _Ready()
	{

	}

	void _on_TouchScreenButton_pressed()
	{
		_on_AnimationPlayer_animation_started("Pressed");
		GD.Print("Touch");
	}

	void _on_AnimationPlayer_animation_started(string name)
	{
		
	}



//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}

