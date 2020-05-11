using Godot;
using System;

public class AnimationControl : AnimationPlayer
{
	public override void _Ready()
	{
		
	}

	public void StartExitAnimation()
	{
		Play("Salir");
	}
	public void StartEnterAnimation()
	{
		if(!IsPlaying()){
			Play("Entrar");
		}
	}
}
