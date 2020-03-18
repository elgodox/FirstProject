using Godot;
using System;

public class HexAnim : Godot.AnimatedSprite
{
    
    public override void _Ready()
    {
        Animation = "idle";
    }
}
