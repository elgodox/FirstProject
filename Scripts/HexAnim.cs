using Godot;
using System;

public class HexAnim : AnimatedSprite
{
    
    public void StartIdle()
    {
        Animation = "Idle";
        Playing = true;
        Play();
    }
}
