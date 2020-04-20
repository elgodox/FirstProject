using Godot;
using System;

public class HexAnim : AnimatedSprite
{
    
    public override void _Ready()
    {
        
    }

    public void StartIdle()
    {
        Animation = "Idle";
        Playing = true;
        Play();
    }
}
