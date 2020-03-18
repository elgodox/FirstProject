using Godot;
using System;

public class AnimationControl : Godot.AnimationPlayer
{
	public override void _Ready()
	{
		if(!IsPlaying()){
			Play("Entrar");
		}
	}

	public void StartExitAnimation()
	{
		GD.Print("Animacion de salir");
		CurrentAnimation = "Salir";
	}
}
