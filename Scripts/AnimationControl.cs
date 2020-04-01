using Godot;
using System;

public class AnimationControl : AnimationPlayer
{
	public override void _Ready()
	{
		
	}

	public void StartExitAnimation()
	{
		CurrentAnimation = "Salir";
	}
	public void StartEnterAnimation()
	{
		if(!IsPlaying()){
			Play("Entrar");
		}
	}
}
