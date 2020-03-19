using Godot;
using System;

public class Control : Godot.Control
{
	[Export] String pathanimationControl;
	[Export] int maxNumPressed;
	AnimationControl animationControl;

	private PackedScene hexManagerScn = ResourceLoader.Load("res://Prefabs/HexManager.tscn") as PackedScene;

	int numHexPressed = 0;
	public override void _Ready()
	{
		animationControl = GetNode<AnimationControl>(pathanimationControl);
	}
	public override void _Process(float delta)
	{
		if (Input.IsActionPressed("ui_A"))
		{
			GD.Print("Spawn HexManager");
            HexManager e;
			e = hexManagerScn.Instance() as HexManager;
            AddChild(e);
		}
	}

	public void _on_Hex_Pressed()
	{
		numHexPressed++;
		if (numHexPressed >= maxNumPressed)
		{
			EjecuteAnimation();
		}
	}

	public void EjecuteAnimation()
	{
		animationControl.StartExitAnimation();
	}
}
