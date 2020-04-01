using Godot;
using System;

public class AnimationControl : AnimationPlayer
{
	public override void _Ready()
	{
		
	}

	public void StartExitAnimation()
	{
		GD.Print("Animacion de salir");
		CurrentAnimation = "Salir";
	}
	public void StartEnterAnimation()
	{
		if(!IsPlaying()){
			Play("Entrar");
		}
	}
}
